function checkLogin() {
    let currentUser = JSON.parse(localStorage.getItem("currentuser"));
    if(currentUser == null || currentUser.userType == 0) {
        document.querySelector("body").innerHTML = `<div class="access-denied-section">
            <img class="access-denied-img" src="~/assets/img/access-denied.webp" alt="">
        </div>`
    } else {
        /*document.getElementById("name-acc").innerHTML = currentUser.fullname;*/
    }
}
//window.onload = checkLogin();

//do sidebar open and close
const menuIconButton = document.querySelector(".menu-icon-btn");
const sidebar = document.querySelector(".sidebar");
menuIconButton.addEventListener("click", () => {
    sidebar.classList.toggle("open");
});

// log out admin user

//let toogleMenu = document.querySelector(".profile");
//let mune = document.querySelector(".profile-cropdown");
//toogleMenu.onclick = function () {
//    mune.classList.toggle("active");
//};


// tab for section
//const sidebars = document.querySelectorAll(".sidebar-list-item.tab-content");
//const sections = document.querySelectorAll(".section");

//for(let i = 0; i < sidebars.length; i++) {
//    sidebars[i].onclick = function () {
//        document.querySelector(".sidebar-list-item.active").classList.remove("active");
//        document.querySelector(".section.active").classList.remove("active");
//        sidebars[i].classList.add("active");
//        sections[i].classList.add("active");
//    };
//}

//const closeBtn = document.querySelectorAll('.section');
//console.log(closeBtn[0])
//for(let i=0;i<closeBtn.length;i++){
//    closeBtn[i].addEventListener('click',(e) => {
//        sidebar.classList.add("open");
//    })
//}

// Get amount product
function getAmoumtProduct() {
    let products = localStorage.getItem("products") ? JSON.parse(localStorage.getItem("products")) : [];
    return products.length;
}

// Get amount user
function getAmoumtUser() {
    let accounts = localStorage.getItem("accounts") ? JSON.parse(localStorage.getItem("accounts")) : [];
    return accounts.filter(item => item.userType == 0).length;
}

// Get amount user
function getMoney() {
    let tongtien = 0;
    let orders = localStorage.getItem("order") ? JSON.parse(localStorage.getItem("order")) : [];
    orders.forEach(item => {
        tongtien += item.tongtien
    });
    return tongtien;
}

//document.getElementById("amount-user").innerHTML = getAmoumtUser();
//document.getElementById("amount-product").innerHTML = getAmoumtProduct();
//document.getElementById("doanh-thu").innerHTML = vnd(getMoney());

// Doi sang dinh dang tien VND
function vnd(price) {
    return price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}

function createId(arr) {
    let id = arr.length;
    let check = arr.find((item) => item.id == id);
    while (check != null) {
        id++;
        check = arr.find((item) => item.id == id);
    }
    return id;
}
// Xóa sản phẩm 
function deleteProduct(id) {
    let products = JSON.parse(localStorage.getItem("products"));
    let index = products.findIndex(item => {
        return item.id == id;
    })
    if (confirm("Bạn có chắc muốn xóa?") == true) {
        products[index].status = 0;
        toast({ title: 'Success', message: 'Xóa sản phẩm thành công !', type: 'success', duration: 3000 });
    }
    localStorage.setItem("products", JSON.stringify(products));
    showProduct();
}

function changeStatusProduct(id) {
    let products = JSON.parse(localStorage.getItem("products"));
    let index = products.findIndex(item => {
        return item.id == id;
    })
    if (confirm("Bạn có chắc chắn muốn hủy xóa?") == true) {
        products[index].status = 1;
        toast({ title: 'Success', message: 'Khôi phục sản phẩm thành công !', type: 'success', duration: 3000 });
    }
    localStorage.setItem("products", JSON.stringify(products));
    showProduct();
}


function getPathImage(path) {
    let patharr = path.split("/");
    return "~/assets/img/products/" + patharr[patharr.length - 1];
}


// Close Popup Modal
let closePopup = document.querySelectorAll(".modal-close");
let modalPopup = document.querySelectorAll(".modal");

for (let i = 0; i < closePopup.length; i++) {
    closePopup[i].onclick = () => {
        modalPopup[i].classList.remove("open");
    };
}



// Format Date
//function formatDate(date) {
//    let fm = new Date(date);
//    let yyyy = fm.getFullYear();
//    let mm = fm.getMonth() + 1;
//    let dd = fm.getDate();
//    if (dd < 10) dd = "0" + dd;
//    if (mm < 10) mm = "0" + mm;
//    return dd + "/" + mm + "/" + yyyy;
//}

// User
//let addAccount = document.getElementById('signup-button');
//let updateAccount = document.getElementById("btn-update-account")

//document.querySelector(".modal.signup .modal-close").addEventListener("click",() => {
//    signUpFormReset();
//})

//function openCreateAccount() {
//    document.querySelector(".signup").classList.add("open");
//    document.querySelectorAll(".edit-account-e").forEach(item => {
//        item.style.display = "none"
//    })
//    document.querySelectorAll(".add-account-e").forEach(item => {
//        item.style.display = "block"
//    })
//}

//function signUpFormReset() {
//    document.getElementById('fullname').value = ""
//    document.getElementById('phone').value = ""
//    document.getElementById('password').value = ""
//    document.querySelector('.form-message-name').innerHTML = '';
//    document.querySelector('.form-message-phone').innerHTML = '';
//    document.querySelector('.form-message-password').innerHTML = '';
//}

//function deleteAcount(phone) {
//    let accounts = JSON.parse(localStorage.getItem('accounts'));
//    let index = accounts.findIndex(item => item.phone == phone);
//    if (confirm("Bạn có chắc muốn xóa?")) {
//        accounts.splice(index, 1)
//    }
//    localStorage.setItem("accounts", JSON.stringify(accounts));
//}

//let indexFlag;
//function editAccount(phone) {
//    document.querySelector(".signup").classList.add("open");
//    document.querySelectorAll(".add-account-e").forEach(item => {
//        item.style.display = "none"
//    })
//    document.querySelectorAll(".edit-account-e").forEach(item => {
//        item.style.display = "block"
//    })
//    let accounts = JSON.parse(localStorage.getItem("accounts"));
//    let index = accounts.findIndex(item => {
//        return item.phone == phone
//    })
//    indexFlag = index;
//    document.getElementById("fullname").value = accounts[index].fullname;
//    document.getElementById("phone").value = accounts[index].phone;
//    document.getElementById("password").value = accounts[index].password;
//    document.getElementById("user-status").checked = accounts[index].status == 1 ? true : false;
//}

//updateAccount.addEventListener("click", (e) => {
//    e.preventDefault();
//    let accounts = JSON.parse(localStorage.getItem("accounts"));
//    let fullname = document.getElementById("fullname").value;
//    let phone = document.getElementById("phone").value;
//    let password = document.getElementById("password").value;
//    if(fullname == "" || phone == "" || password == "") {
//        toast({ title: 'Chú ý', message: 'Vui lòng nhập đầy đủ thông tin !', type: 'warning', duration: 3000 });
//    } else {
//        accounts[indexFlag].fullname = document.getElementById("fullname").value;
//        accounts[indexFlag].phone = document.getElementById("phone").value;
//        accounts[indexFlag].password = document.getElementById("password").value;
//        accounts[indexFlag].status = document.getElementById("user-status").checked ? true : false;
//        localStorage.setItem("accounts", JSON.stringify(accounts));
//        toast({ title: 'Thành công', message: 'Thay đổi thông tin thành công !', type: 'success', duration: 3000 });
//        document.querySelector(".signup").classList.remove("open");
//        signUpFormReset();
//    }
//})

//addAccount.addEventListener("click", (e) => {
//    e.preventDefault();
//    let fullNameUser = document.getElementById('fullname').value;
//    let phoneUser = document.getElementById('phone').value;
//    let passwordUser = document.getElementById('password').value;
//        // Check validate
//        let fullNameIP = document.getElementById('fullname');
//        let formMessageName = document.querySelector('.form-message-name');
//        let formMessagePhone = document.querySelector('.form-message-phone');
//        let formMessagePassword = document.querySelector('.form-message-password');
    
//        if (fullNameUser.length == 0) {
//            formMessageName.innerHTML = 'Vui lòng nhập họ vâ tên';
//            fullNameIP.focus();
//        } else if (fullNameUser.length < 3) {
//            fullNameIP.value = '';
//            formMessageName.innerHTML = 'Vui lòng nhập họ và tên lớn hơn 3 kí tự';
//        }
        
//        if (phoneUser.length == 0) {
//            formMessagePhone.innerHTML = 'Vui lòng nhập vào số điện thoại';
//        } else if (phoneUser.length != 10) {
//            formMessagePhone.innerHTML = 'Vui lòng nhập vào số điện thoại 10 số';
//            document.getElementById('phone').value = '';
//        }
        
//        if (passwordUser.length == 0) {
//            formMessagePassword.innerHTML = 'Vui lòng nhập mật khẩu';
//        } else if (passwordUser.length < 6) {
//            formMessagePassword.innerHTML = 'Vui lòng nhập mật khẩu lớn hơn 6 kí tự';
//            document.getElementById('password').value = '';
//        }

//    if (fullNameUser && phoneUser && passwordUser) {
//        let user = {
//            fullname: fullNameUser,
//            phone: phoneUser,
//            password: passwordUser,
//            address: '',
//            email: '',
//            status: 1,
//            join: new Date(),
//            cart: [],
//            userType: 0
//        }
//        console.log(user);
//        let accounts = localStorage.getItem('accounts') ? JSON.parse(localStorage.getItem('accounts')) : [];
//        let checkloop = accounts.some(account => {
//            return account.phone == user.phone;
//        })
//        if (!checkloop) {
//            accounts.push(user);
//            localStorage.setItem('accounts', JSON.stringify(accounts));
//            toast({ title: 'Thành công', message: 'Tạo thành công tài khoản !', type: 'success', duration: 3000 });
//            document.querySelector(".signup").classList.remove("open");
//            signUpFormReset();
//        } else {
//            toast({ title: 'Cảnh báo !', message: 'Tài khoản đã tồn tại !', type: 'error', duration: 3000 });
//        }
//    }
//})

//document.getElementById("logout-acc").addEventListener('click', (e) => {
//    e.preventDefault();
//    localStorage.removeItem("currentuser");
//    window.location = "/";
//})