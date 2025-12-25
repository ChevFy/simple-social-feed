const state = {
	token: localStorage.getItem('authToken') || null
};

// Validation helpers
function clearError(fieldId) {
	const errorEl = document.getElementById(fieldId);
	if (errorEl) errorEl.textContent = '';
}

function showError(fieldId, message) {
	const errorEl = document.getElementById(fieldId);
	if (errorEl) {
		errorEl.textContent = message;
		errorEl.style.color = '#dc2626';
	}
}

function validateRequired(value, fieldName, errorId) {
	if (!value || value.trim() === '') {
		showError(errorId, `${fieldName} is required`);
		return false;
	}
	clearError(errorId);
	return true;
}

function validateMinLength(value, minLength, fieldName, errorId) {
	if (value && value.length < minLength) {
		showError(errorId, `${fieldName} must be at least ${minLength} characters`);
		return false;
	}
	clearError(errorId);
	return true;
}

function validatePasswordMatch(password, confirmPassword, errorId) {
	if (password !== confirmPassword) {
		showError(errorId, 'Passwords do not match');
		return false;
	}
	clearError(errorId);
	return true;
}

function showMessage(targetId, message, type = 'info') {
	const el = document.getElementById(targetId);
	if (!el) return;
	el.textContent = message || '';
	el.className = `status ${type}`.trim();
}

function updateAuthUI() {
	const isAuthed = Boolean(state.token);
	const status = document.getElementById('authStatus');
	if (status) {
		status.textContent = isAuthed ? 'Signed in' : 'Guest';
		status.className = `pill ${isAuthed ? 'pill-strong' : 'pill-muted'}`;
	}

	document.querySelectorAll('[data-auth-only]').forEach(el => {
		el.style.display = isAuthed ? '' : 'none';
	});

	document.querySelectorAll('[data-guest-only]').forEach(el => {
		el.style.display = isAuthed ? 'none' : '';
	});

	const logoutBtn = document.getElementById('logoutBtn');
	if (logoutBtn) {
		logoutBtn.style.display = isAuthed ? '' : 'none';
	}
}

function setToken(token) {
	state.token = token;
	if (token) {
		localStorage.setItem('authToken', token);
	} else {
		localStorage.removeItem('authToken');
	}
	updateAuthUI();
}

function authHeaders() {
	return state.token ? { Authorization: `Bearer ${state.token}` } : {};
}

function formatDate(value) {
	if (!value) return 'Not set';
	const d = new Date(value);
	return Number.isNaN(d.getTime()) ? 'Not set' : d.toLocaleString();
}

async function handleRegister(event) {
	event.preventDefault();
	
	const username = document.getElementById('registerUsername').value.trim();
	const password = document.getElementById('registerPassword').value;
	const confirmPassword = document.getElementById('registerConfirm').value;

	// Clear previous errors
	clearError('registerUsernameError');
	clearError('registerPasswordError');
	clearError('registerConfirmError');

	// Validate
	let isValid = true;
	if (!validateRequired(username, 'Username', 'registerUsernameError')) isValid = false;
	if (!validateMinLength(username, 3, 'Username', 'registerUsernameError')) isValid = false;
	if (!validateRequired(password, 'Password', 'registerPasswordError')) isValid = false;
	if (!validateMinLength(password, 6, 'Password', 'registerPasswordError')) isValid = false;
	if (!validateRequired(confirmPassword, 'Confirm password', 'registerConfirmError')) isValid = false;
	if (!validatePasswordMatch(password, confirmPassword, 'registerConfirmError')) isValid = false;

	if (!isValid) {
		showMessage('registerStatus', 'Please fix the errors above', 'error');
		return;
	}

	const payload = {
		Username: username,
		Password: password,
		ConfirmPassword: confirmPassword
	};

	showMessage('registerStatus', 'Registering...', 'info');
	try {
		const res = await fetch('/api/Auth/register', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(payload)
		});

		if (res.ok) {
			showMessage('registerStatus', 'Registered successfully! Redirecting to login...', 'success');
			setTimeout(() => window.location.href = '/login', 1500);
			return;
		}
		const errorText = await res.text();
		showMessage('registerStatus', errorText || 'Registration failed.', 'error');
	} catch (err) {
		showMessage('registerStatus', 'Network error while registering.', 'error');
	}
}

async function handleLogin(event) {
	event.preventDefault();
	
	const username = document.getElementById('loginUsername').value.trim();
	const password = document.getElementById('loginPassword').value;

	// Clear previous errors
	clearError('usernameError');
	clearError('passwordError');

	// Validate
	let isValid = true;
	if (!validateRequired(username, 'Username', 'usernameError')) isValid = false;
	if (!validateRequired(password, 'Password', 'passwordError')) isValid = false;

	if (!isValid) {
		showMessage('loginStatus', 'Please fix the errors above', 'error');
		return;
	}

	const payload = {
		Username: username,
		Password: password
	};

	showMessage('loginStatus', 'Signing in...', 'info');
	try {
		const res = await fetch('/api/Auth/login', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(payload)
		});

		if (res.ok) {
			const data = await res.json();
			setToken(data.token);
			showMessage('loginStatus', 'Signed in successfully! Redirecting...', 'success');
			setTimeout(() => window.location.href = '/posts', 1000);
			return;
		}
		const errorText = await res.text();
		showMessage('loginStatus', errorText || 'Login failed.', 'error');
	} catch (err) {
		showMessage('loginStatus', 'Network error while signing in.', 'error');
	}
}

function handleLogout() {
	setToken(null);
	showMessage('loginStatus', 'Signed out.', 'info');
	window.location.href = '/';
}

async function createPost(event) {
	event.preventDefault();
	
	if (!state.token) {
		showMessage('createPostStatus', 'Please sign in first.', 'error');
		setTimeout(() => window.location.href = '/login', 1500);
		return;
	}

	const title = document.getElementById('postTitle').value.trim();
	const description = document.getElementById('postDescription').value.trim();

	// Clear previous errors
	clearError('postTitleError');
	clearError('postDescriptionError');

	// Validate
	let isValid = true;
	if (!validateRequired(title, 'Title', 'postTitleError')) isValid = false;
	if (!validateMinLength(title, 3, 'Title', 'postTitleError')) isValid = false;

	if (!isValid) {
		showMessage('createPostStatus', 'Please fix the errors above', 'error');
		return;
	}

	const payload = {
		Title: title,
		Describtion: description
	};

	showMessage('createPostStatus', 'Publishing...', 'info');
	try {
		const res = await fetch('/api/User/post', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json', ...authHeaders() },
			body: JSON.stringify(payload)
		});

		if (res.ok) {
			showMessage('createPostStatus', 'Posted successfully! Redirecting...', 'success');
			document.getElementById('postTitle').value = '';
			document.getElementById('postDescription').value = '';
			setTimeout(() => window.location.href = '/my-posts', 1000);
			return;
		}
		
		if (res.status === 401) {
			showMessage('createPostStatus', 'Session expired. Please sign in again.', 'error');
			setToken(null);
			setTimeout(() => window.location.href = '/login', 1500);
			return;
		}
		
		const errorText = await res.text();
		showMessage('createPostStatus', errorText || 'Could not post.', 'error');
	} catch (err) {
		showMessage('createPostStatus', 'Network error while posting.', 'error');
	}
}

async function fetchFeed() {
	showMessage('feedStatus', 'Loading feed...', 'info');
	const container = document.getElementById('feedList');
	if (container) container.innerHTML = '';
	
	const countEl = document.getElementById('postCount');
	if (countEl) countEl.textContent = 'Loading...';
	
	try {
		const res = await fetch('/api/post');
		if (res.status === 404) {
			showMessage('feedStatus', 'No posts yet. Be the first to post!', 'info');
			if (countEl) countEl.textContent = '0 posts';
			return;
		}
		if (!res.ok) {
			showMessage('feedStatus', 'Could not load feed.', 'error');
			if (countEl) countEl.textContent = 'Error';
			return;
		}
		const posts = await res.json();
		renderPosts(posts, 'feedList', false);
		showMessage('feedStatus', '', 'info');
		if (countEl) countEl.textContent = `${posts.length} post${posts.length !== 1 ? 's' : ''}`;
	} catch (err) {
		showMessage('feedStatus', 'Network error while loading feed.', 'error');
		if (countEl) countEl.textContent = 'Error';
	}
}

function renderPosts(posts, targetId, allowDelete = false) {
	const container = document.getElementById(targetId);
	if (!container) return;
	container.innerHTML = '';

	if (!posts || posts.length === 0) {
		container.innerHTML = '<p class="post-meta">Nothing here yet.</p>';
		return;
	}

	posts.forEach(post => {
		const item = document.createElement('div');
		item.className = 'post-item';

		const content = document.createElement('div');
		const title = document.createElement('p');
		title.className = 'post-title';
		title.textContent = post.title || post.Title;

		const desc = document.createElement('p');
		desc.className = 'post-desc';
		desc.textContent = post.describtion || post.Describtion || '';

		const meta = document.createElement('div');
		meta.className = 'post-meta';
		const username = post.username || post.Username || 'Unknown';
		meta.textContent = `${username} • ${formatDate(post.dateCreated || post.DateCreated)}`;

		content.appendChild(title);
		if (desc.textContent) content.appendChild(desc);
		content.appendChild(meta);

		item.appendChild(content);

		if (allowDelete) {
			const actions = document.createElement('div');
			actions.className = 'post-actions';
			const del = document.createElement('button');
			del.className = 'ghost-btn';
			del.textContent = 'Delete';
			del.addEventListener('click', () => deletePost(post.id || post.Id));
			actions.appendChild(del);
			item.appendChild(actions);
		}

		container.appendChild(item);
	});
}

async function fetchMyPosts() {
	if (!state.token) {
		showMessage('myPostsStatus', 'Please sign in to see your posts.', 'error');
		setTimeout(() => window.location.href = '/login', 1500);
		return;
	}
	
	showMessage('myPostsStatus', 'Loading your posts...', 'info');
	const container = document.getElementById('myPostsList');
	if (container) container.innerHTML = '';
	
	const countEl = document.getElementById('myPostCount');
	if (countEl) countEl.textContent = 'Loading...';
	
	try {
		const res = await fetch('/api/User/posts', {
			headers: { ...authHeaders() }
		});
		
		if (res.status === 401) {
			showMessage('myPostsStatus', 'Session expired. Please sign in again.', 'error');
			setToken(null);
			setTimeout(() => window.location.href = '/login', 1500);
			return;
		}
		
		if (!res.ok) {
			showMessage('myPostsStatus', 'Could not load your posts.', 'error');
			if (countEl) countEl.textContent = 'Error';
			return;
		}
		
		const posts = await res.json();
		renderPosts(posts, 'myPostsList', true);
		showMessage('myPostsStatus', '', 'info');
		if (countEl) countEl.textContent = `${posts.length} post${posts.length !== 1 ? 's' : ''}`;
	} catch (err) {
		showMessage('myPostsStatus', 'Network error while loading your posts.', 'error');
		if (countEl) countEl.textContent = 'Error';
	}
}

async function deletePost(id) {
	if (!id) return;
	if (!state.token) {
		showMessage('myPostsStatus', 'Please sign in first.', 'error');
		setTimeout(() => window.location.href = '/login', 1500);
		return;
	}
	
	if (!confirm('Are you sure you want to delete this post?')) {
		return;
	}
	
	showMessage('myPostsStatus', 'Deleting...', 'info');
	try {
		const res = await fetch(`/api/User/post?id=${encodeURIComponent(id)}`, {
			method: 'DELETE',
			headers: { ...authHeaders() }
		});
		
		if (res.status === 401) {
			showMessage('myPostsStatus', 'Session expired. Please sign in again.', 'error');
			setToken(null);
			setTimeout(() => window.location.href = '/login', 1500);
			return;
		}
		
		if (!res.ok) {
			const errorText = await res.text();
			showMessage('myPostsStatus', errorText || 'Delete failed.', 'error');
			return;
		}
		
		showMessage('myPostsStatus', 'Deleted successfully.', 'success');
		await fetchMyPosts();
	} catch (err) {
		showMessage('myPostsStatus', 'Network error while deleting.', 'error');
	}
}

async function changePassword(event) {
	event.preventDefault();
	
	if (!state.token) {
		showMessage('changePasswordStatus', 'Please sign in first.', 'error');
		setTimeout(() => window.location.href = '/login', 1500);
		return;
	}
	
	const oldPassword = document.getElementById('oldPassword').value;
	const newPassword = document.getElementById('newPassword').value;
	const confirmNewPassword = document.getElementById('confirmNewPassword').value;

	// Clear previous errors
	clearError('oldPasswordError');
	clearError('newPasswordError');
	clearError('confirmNewPasswordError');

	// Validate
	let isValid = true;
	if (!validateRequired(oldPassword, 'Old password', 'oldPasswordError')) isValid = false;
	if (!validateRequired(newPassword, 'New password', 'newPasswordError')) isValid = false;
	if (!validateMinLength(newPassword, 6, 'New password', 'newPasswordError')) isValid = false;
	if (!validateRequired(confirmNewPassword, 'Confirm new password', 'confirmNewPasswordError')) isValid = false;
	if (!validatePasswordMatch(newPassword, confirmNewPassword, 'confirmNewPasswordError')) isValid = false;

	if (!isValid) {
		showMessage('changePasswordStatus', 'Please fix the errors above', 'error');
		return;
	}

	const payload = {
		OldPassword: oldPassword,
		NewPassword: newPassword,
		ConfirmNewPassword: confirmNewPassword
	};

	showMessage('changePasswordStatus', 'Updating password...', 'info');
	try {
		const res = await fetch('/api/Auth/changepassword', {
			method: 'PUT',
			headers: { 'Content-Type': 'application/json', ...authHeaders() },
			body: JSON.stringify(payload)
		});

		if (res.status === 401) {
			showMessage('changePasswordStatus', 'Session expired. Please sign in again.', 'error');
			setToken(null);
			setTimeout(() => window.location.href = '/login', 1500);
			return;
		}

		if (res.ok) {
			showMessage('changePasswordStatus', 'Password updated successfully!', 'success');
			document.getElementById('oldPassword').value = '';
			document.getElementById('newPassword').value = '';
			document.getElementById('confirmNewPassword').value = '';
			return;
		}
		
		const errorText = await res.text();
		showMessage('changePasswordStatus', errorText || 'Could not update password.', 'error');
	} catch (err) {
		showMessage('changePasswordStatus', 'Network error while updating password.', 'error');
	}
}

function wireUp() {
	const registerForm = document.getElementById('registerForm');
	if (registerForm) {
		registerForm.addEventListener('submit', handleRegister);
	}

	const loginForm = document.getElementById('loginForm');
	if (loginForm) {
		loginForm.addEventListener('submit', handleLogin);
	}

	const logoutBtn = document.getElementById('logoutBtn');
	if (logoutBtn) {
		logoutBtn.addEventListener('click', handleLogout);
	}

	const createPostForm = document.getElementById('createPostForm');
	if (createPostForm) {
		createPostForm.addEventListener('submit', createPost);
	}

	const refreshFeed = document.getElementById('refreshFeed');
	if (refreshFeed) {
		refreshFeed.addEventListener('click', fetchFeed);
	}

	const refreshMyPosts = document.getElementById('refreshMyPosts');
	if (refreshMyPosts) {
		refreshMyPosts.addEventListener('click', fetchMyPosts);
	}

	const changePasswordForm = document.getElementById('changePasswordForm');
	if (changePasswordForm) {
		changePasswordForm.addEventListener('submit', changePassword);
	}
}

window.addEventListener('DOMContentLoaded', async () => {
	updateAuthUI();
	wireUp();
	
	// Auto-load data based on current page
	const path = window.location.pathname;
	
	if (path === '/posts') {
		await fetchFeed();
	} else if (path === '/my-posts') {
		if (state.token) {
			await fetchMyPosts();
		} else {
			showMessage('myPostsStatus', 'Please sign in to see your posts.', 'error');
			setTimeout(() => window.location.href = '/login', 1500);
		}
	} else if (path === '/create-post') {
		if (!state.token) {
			showMessage('createPostStatus', 'Please sign in to create posts.', 'error');
			setTimeout(() => window.location.href = '/login', 1500);
		}
	}
});
