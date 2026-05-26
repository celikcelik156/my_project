const registerButton = document.getElementById("register");
const loginButton = document.getElementById("login");
const container = document.getElementById("container");

if (registerButton && loginButton && container) {
    registerButton.addEventListener("click", () => {
        container.classList.add("right-panel-active");
    });

    loginButton.addEventListener("click", () => {
        container.classList.remove("right-panel-active");
    });
}

const regForm = document.querySelector('.register-container form');
const username = document.getElementById('username');
const email = document.getElementById('email');
const password = document.getElementById('password');

const lgForm = document.querySelector('.form-lg');
const lgEmail = document.getElementById('login-email');
const lgPassword = document.getElementById('passwordInput');

function showError(input, message) {
    const formControl = input.parentElement;
    if (formControl.classList.contains('form-control')) {
        formControl.className = 'form-control error';
    } else {
        formControl.className = 'form-control2 error';
    }
    const small = formControl.querySelector('small');
    small.innerText = message;
}

function showSuccess(input) {
    const formControl = input.parentElement;
    if (formControl.classList.contains('form-control')) {
        formControl.className = 'form-control success';
    } else {
        formControl.className = 'form-control2 success';
    }
    const small = formControl.querySelector('small');
    small.innerText = '';
}

function checkRequired(inputArr) {
    let hasError = false;
    inputArr.forEach(function (input) {
        if (input.value.trim() === '') {
            showError(input, `Bu alan zorunludur.`);
            hasError = true;
        } else {
            showSuccess(input);
        }
    });
    return hasError;
}

if (regForm && username && email && password) {
    regForm.addEventListener('submit', function (e) {
        console.log('Register form submit event triggered');
        const hasError = checkRequired([username, email, password]);
        if (hasError) {
            console.log('Register form validation failed');
            e.preventDefault();
            return false;
        }
        console.log('Register form validation passed, submitting to server...');
    });
} else {
    console.error('Register form elements not found');
}

if (lgForm && lgEmail && lgPassword) {
    lgForm.addEventListener('submit', function (e) {
        console.log('Login form submit event triggered');
        const hasError = checkRequired([lgEmail, lgPassword]);
        if (hasError) {
            console.log('Login form validation failed');
            e.preventDefault();
            return false;
        }
        console.log('Login form validation passed, submitting to server...');
    });
} else {
    console.error('Login form elements not found');
}