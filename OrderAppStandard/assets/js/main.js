// Doi sang dinh dang tien VND
function vnd(price) {
    return price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}

// Close popup 
const body = document.querySelector("body");
let modalContainer = document.querySelectorAll('.modal');
let modalBox = document.querySelectorAll('.mdl-cnt');
let formLogSign = document.querySelector('.forms');

// Click vùng ngoài sẽ tắt Popup
modalContainer.forEach(item => {
    item.addEventListener('click', closeModal);
});

modalBox.forEach(item => {
    item.addEventListener('click', function (event) {
        event.stopPropagation();
    })
});

function closeModal() {
    modalContainer.forEach(item => {
        item.classList.remove('open');
    });
    console.log(modalContainer)
    body.style.overflow = "auto";
}

//function increasingNumber(e) {
//    let qty = e.parentNode.querySelector('.input-qty');
//    if (parseInt(qty.value) < qty.max) {
//        qty.value = parseInt(qty.value) + 1;
//    } else {
//        qty.value = qty.max;
//    }
//}

//function decreasingNumber(e) {
//    let qty = e.parentNode.querySelector('.input-qty');
//    if (qty.value > qty.min) {
//        qty.value = parseInt(qty.value) - 1;
//    } else {
//        qty.value = qty.min;
//    }
//}

//Xem chi tiet san pham
function detailProduct(index) {
    let modal = document.querySelector('.modal.product-detail');
    let products = JSON.parse(localStorage.getItem('products'));
    event.preventDefault();
    let infoProduct = products.find(sp => {
        return sp.id === index;
    })
    let modalHtml = `<div class="modal-header">
    <img class="product-image" src="${infoProduct.img}" alt="">
    </div>
    <div class="modal-body">
        <h2 class="product-title">${infoProduct.title}</h2>
        <div class="product-control">
            <div class="priceBox">
                <span class="current-price">${vnd(infoProduct.price)}</span>
            </div>
            <div class="buttons_added">
                <input class="minus is-form" type="button" value="-" onclick="decreasingNumber(this)">
                <input class="input-qty" max="100" min="1" name="" type="number" value="1">
                <input class="plus is-form" type="button" value="+" onclick="increasingNumber(this)">
            </div>
        </div>
        <p class="product-description">${infoProduct.desc}</p>
    </div>
    <div class="notebox">
            <p class="notebox-title">Ghi chú</p>
            <textarea class="text-note" id="popup-detail-note" placeholder="Nhập thông tin cần lưu ý..."></textarea>
    </div>
    <div class="modal-footer">
        <div class="price-total">
            <span class="thanhtien">Thành tiền</span>
            <span class="price">${vnd(infoProduct.price)}</span>
        </div>
        <div class="modal-footer-control">
            <button class="button-dathangngay" data-product="${infoProduct.id}">Đặt hàng ngay</button>
            <button class="button-dat" id="add-cart" onclick="animationCart()"><i class="fa-light fa-basket-shopping"></i></button>
        </div>
    </div>`;
    document.querySelector('#product-detail-content').innerHTML = modalHtml;
    modal.classList.add('open');
    body.style.overflow = "hidden";
    //Cap nhat gia tien khi tang so luong san pham
    let tgbtn = document.querySelectorAll('.is-form');
    let qty = document.querySelector('.product-control .input-qty');
    let priceText = document.querySelector('.price');
    tgbtn.forEach(element => {
        element.addEventListener('click', () => {
            let price = infoProduct.price * parseInt(qty.value);
            priceText.innerHTML = vnd(price);
        });
    });
    // Them san pham vao gio hang
    let productbtn = document.querySelector('.button-dat');
    productbtn.addEventListener('click', (e) => {
        if (localStorage.getItem('currentuser')) {
            addCart(infoProduct.id);
        } else {
            toast({ title: 'Warning', message: 'Chưa đăng nhập tài khoản !', type: 'warning', duration: 3000 });
        }

    })
    // Mua ngay san pham
    dathangngay();
}

function animationCart() {
    document.querySelector(".count-product-cart").style.animation = "slidein ease 1s"
    setTimeout(()=>{
        document.querySelector(".count-product-cart").style.animation = "none"
    },1000)
}

// Them SP vao gio hang
function addCart(index) {
    let currentuser = localStorage.getItem('currentuser') ? JSON.parse(localStorage.getItem('currentuser')) : [];
    let soluong = document.querySelector('.input-qty').value;
    let popupDetailNote = document.querySelector('#popup-detail-note').value;
    let note = popupDetailNote == "" ? "Không có ghi chú" : popupDetailNote;
    let productcart = {
        id: index,
        soluong: parseInt(soluong),
        note: note
    }
    let vitri = currentuser.cart.findIndex(item => item.id == productcart.id);
    if (vitri == -1) {
        currentuser.cart.push(productcart);
    } else {
        currentuser.cart[vitri].soluong = parseInt(currentuser.cart[vitri].soluong) + parseInt(productcart.soluong);
    }
    localStorage.setItem('currentuser', JSON.stringify(currentuser));
    //updateAmount();
    closeModal();
    // toast({ title: 'Success', message: 'Thêm thành công sản phẩm vào giỏ hàng', type: 'success', duration: 3000 });
}

//Show gio hang
function showCart() {
    if (localStorage.getItem('currentuser') != null) {
        let currentuser = JSON.parse(localStorage.getItem('currentuser'));
        if (currentuser.cart.length != 0) {
            document.querySelector('.gio-hang-trong').style.display = 'none';
            document.querySelector('button.thanh-toan').classList.remove('disabled');
            let productcarthtml = '';
            currentuser.cart.forEach(item => {
                let product = getProduct(item);
                productcarthtml += `<li class="cart-item" data-id="${product.id}">
                <div class="cart-item-info">
                    <p class="cart-item-title">
                        ${product.title}
                    </p>
                    <span class="cart-item-price price" data-price="${product.price}">
                    ${vnd(parseInt(product.price))}
                    </span>
                </div>
                <p class="product-note"><i class="fa-light fa-pencil"></i><span>${product.note}</span></p>
                <div class="cart-item-control">
                    <button class="cart-item-delete" onclick="deleteCartItem(${product.id},this)">Xóa</button>
                    <div class="buttons_added">
                        <input class="minus is-form" type="button" value="-" onclick="decreasingNumber(this)">
                        <input class="input-qty" max="100" min="1" name="" type="number" value="${product.soluong}">
                        <input class="plus is-form" type="button" value="+" onclick="increasingNumber(this)">
                    </div>
                </div>
            </li>`
            });
            document.querySelector('.cart-list').innerHTML = productcarthtml;
            updateCartTotal();
            saveAmountCart();
        } else {
            document.querySelector('.gio-hang-trong').style.display = 'flex'
        }
    }
    let modalCart = document.querySelector('.modal-cart');
    let containerCart = document.querySelector('.cart-container');
    let themmon = document.querySelector('.them-mon');
    modalCart.onclick = function () {
        closeCart();
    }
    themmon.onclick = function () {
        closeCart();
    }
    containerCart.addEventListener('click', (e) => {
        e.stopPropagation();
    })
}

// Delete cart item
function deleteCartItem(id, el) {
    let cartParent = el.parentNode.parentNode;
    cartParent.remove();
    let currentUser = JSON.parse(localStorage.getItem('currentuser'));
    let vitri = currentUser.cart.findIndex(item => item.id = id)
    currentUser.cart.splice(vitri, 1);

    // Nếu trống thì hiển thị giỏ hàng trống
    if (currentUser.cart.length == 0) {
        document.querySelector('.gio-hang-trong').style.display = 'flex';
        document.querySelector('button.thanh-toan').classList.add('disabled');
    }
    localStorage.setItem('currentuser', JSON.stringify(currentUser));
    updateCartTotal();
}

//Update cart total
function updateCartTotal() {
    document.querySelector('.text-price').innerText = vnd(getCartTotal());
}

// Lay tong tien don hang
function getCartTotal() {
    let currentUser = JSON.parse(localStorage.getItem('currentuser'));
    let tongtien = 0;
    if (currentUser != null) {
        currentUser.cart.forEach(item => {
            let product = getProduct(item);
            tongtien += (parseInt(product.soluong) * parseInt(product.price));
        });
    }
    return tongtien;
}

// Get Product 
function getProduct(item) {
    let products = JSON.parse(localStorage.getItem('products'));
    let infoProductCart = products.find(sp => item.id == sp.id)
    let product = {
        ...infoProductCart,
        ...item
    }
    return product;
}

//window.onload = updateAmount();
window.onload = updateCartTotal();

// Lay so luong hang

function getAmountCart() {
    let currentuser = JSON.parse(localStorage.getItem('currentuser'))
    let amount = 0;
    currentuser.cart.forEach(element => {
        amount += parseInt(element.soluong);
    });
    return amount;
}

//Update Amount Cart 
//function updateAmount() {
//    if (localStorage.getItem('currentuser') != null) {
//        let amount = getAmountCart();
//        document.querySelector('.count-product-cart').innerText = amount;
//    }
//}

// Save Cart Info
function saveAmountCart() {
    let cartAmountbtn = document.querySelectorAll(".cart-item-control .is-form");
    let listProduct = document.querySelectorAll('.cart-item');
    let currentUser = JSON.parse(localStorage.getItem('currentuser'));
    cartAmountbtn.forEach((btn, index) => {
        btn.addEventListener('click', () => {
            let id = listProduct[parseInt(index / 2)].getAttribute("data-id");
            let productId = currentUser.cart.find(item => {
                return item.id == id;
            });
            productId.soluong = parseInt(listProduct[parseInt(index / 2)].querySelector(".input-qty").value);
            localStorage.setItem('currentuser', JSON.stringify(currentUser));
            updateCartTotal();
        })
    });
}

// Open & Close Cart
function openCart() {
    showCart();
    document.querySelector('.modal-cart').classList.add('open');
    body.style.overflow = "hidden";
}

function closeCart() {
    document.querySelector('.modal-cart').classList.remove('open');
    body.style.overflow = "auto";
    //updateAmount();
}

document.querySelector(".form-search-input").addEventListener("click",(e) => {
    e.preventDefault();
    document.getElementById("home-service").scrollIntoView();
})

function closeSearchAdvanced() {
    document.querySelector(".advanced-search").classList.toggle("open");
}

//Open Search Mobile 
function openSearchMb() {
    document.querySelector(".header-middle-left").style.display = "none";
    document.querySelector(".header-middle-center").style.display = "block";
    document.querySelector(".header-middle-right-item.close").style.display = "block";
    let liItem = document.querySelectorAll(".header-middle-right-item.open");
    for(let i = 0; i < liItem.length; i++) {
        liItem[i].style.setProperty("display", "none", "important")
    }
}

//Close Search Mobile 
function closeSearchMb() {
    document.querySelector(".header-middle-left").style.display = "block";
    document.querySelector(".header-middle-center").style.display = "none";
    document.querySelector(".header-middle-right-item.close").style.display = "none";
    let liItem = document.querySelectorAll(".header-middle-right-item.open");
    for(let i = 0; i < liItem.length; i++) {
        liItem[i].style.setProperty("display", "block", "important")
    }
}

//Signup && Login Form

// Chuyen doi qua lai SignUp & Login 
let signup = document.querySelector('.signup-link');
let login = document.querySelector('.login-link');
let container = document.querySelector('.signup-login .modal-container');
login.addEventListener('click', () => {
    container.classList.add('active');
})

signup.addEventListener('click', () => {
    container.classList.remove('active');
})

//let signupbtn = document.getElementById('signup');
//let loginbtn = document.getElementById('login');
//let formsg = document.querySelector('.modal.signup-login')
//signupbtn.addEventListener('click', () => {
//    formsg.classList.add('open');
//    container.classList.remove('active');
//    body.style.overflow = "hidden";
//})

//loginbtn.addEventListener('click', () => {
//    document.querySelector('.form-message-check-login').innerHTML = '';
//    formsg.classList.add('open');
//    container.classList.add('active');
//    body.style.overflow = "hidden";
//})

// Dang nhap & Dang ky


// Chuyển đổi trang chủ và trang thông tin tài khoản
function myAccount() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    document.getElementById('trangchu').classList.add('hide');
    document.getElementById('order-history').classList.remove('open');
    document.getElementById('account-user').classList.add('open');
    userInfo();
}

// Chuyển đổi trang chủ và trang xem lịch sử đặt hàng 
function orderHistory() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    //document.getElementById('account-user').classList.remove('open');
    document.getElementById('trangchu').classList.add('hide');
    document.getElementById('order-history').classList.add('open');
    //renderOrderProduct();
}

function emailIsValid(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
}

function userInfo() {
    let user = JSON.parse(localStorage.getItem('currentuser'));
    document.getElementById('infoname').value = user.fullname;
    document.getElementById('infophone').value = user.phone;
    document.getElementById('infoemail').value = user.email;
    document.getElementById('infoaddress').value = user.address;
    if (user.email == undefined) {
        infoemail.value = '';
    }
    if (user.address == undefined) {
        infoaddress.value = '';
    }
}

// Thay doi thong tin
function changeInformation() {
    let accounts = JSON.parse(localStorage.getItem('accounts'));
    let user = JSON.parse(localStorage.getItem('currentuser'));
    let infoname = document.getElementById('infoname');
    let infoemail = document.getElementById('infoemail');
    let infoaddress = document.getElementById('infoaddress');

    user.fullname = infoname.value;
    if (infoemail.value.length > 0) {
        if (!emailIsValid(infoemail.value)) {
            document.querySelector('.inforemail-error').innerHTML = 'Vui lòng nhập lại email!';
            infoemail.value = '';
        } else {
            user.email = infoemail.value;
        }
    }

    if (infoaddress.value.length > 0) {
        user.address = infoaddress.value;
    }

    let vitri = accounts.findIndex(item => item.phone == user.phone)

    accounts[vitri].fullname = user.fullname;
    accounts[vitri].email = user.email;
    accounts[vitri].address = user.address;
    localStorage.setItem('currentuser', JSON.stringify(user));
    localStorage.setItem('accounts', JSON.stringify(accounts));
    kiemtradangnhap();
    toast({ title: 'Success', message: 'Cập nhật thông tin thành công !', type: 'success', duration: 3000 });
}

// Đổi mật khẩu 
function changePassword() {
    let currentUser = JSON.parse(localStorage.getItem("currentuser"));
    let passwordCur = document.getElementById('password-cur-info');
    let passwordAfter = document.getElementById('password-after-info');
    let passwordConfirm = document.getElementById('password-comfirm-info');
    let check = true;
    if (passwordCur.value.length == 0) {
        document.querySelector('.password-cur-info-error').innerHTML = 'Vui lòng nhập mật khẩu hiện tại';
        check = false;
    } else {
        document.querySelector('.password-cur-info-error').innerHTML = '';
    }

    if (passwordAfter.value.length == 0) {
        document.querySelector('.password-after-info-error').innerHTML = 'Vui lòn nhập mật khẩu mới';
        check = false;
    } else {
        document.querySelector('.password-after-info-error').innerHTML = '';
    }

    if (passwordConfirm.value.length == 0) {
        document.querySelector('.password-after-comfirm-error').innerHTML = 'Vui lòng nhập mật khẩu xác nhận';
        check = false;
    } else {
        document.querySelector('.password-after-comfirm-error').innerHTML = '';
    }

    if (check == true) {
        if (passwordCur.value.length > 0) {
            if (passwordCur.value == currentUser.password) {
                document.querySelector('.password-cur-info-error').innerHTML = '';
                if (passwordAfter.value.length > 0) {
                    if (passwordAfter.value.length < 6) {
                        document.querySelector('.password-after-info-error').innerHTML = 'Vui lòng nhập mật khẩu mới có số  kí tự lớn hơn bằng 6';
                    } else {
                        document.querySelector('.password-after-info-error').innerHTML = '';
                        if (passwordConfirm.value.length > 0) {
                            if (passwordConfirm.value == passwordAfter.value) {
                                document.querySelector('.password-after-comfirm-error').innerHTML = '';
                                currentUser.password = passwordAfter.value;
                                localStorage.setItem('currentuser', JSON.stringify(currentUser));
                                let userChange = JSON.parse(localStorage.getItem('currentuser'));
                                let accounts = JSON.parse(localStorage.getItem('accounts'));
                                let accountChange = accounts.find(acc => {
                                    return acc.phone = userChange.phone;
                                })
                                accountChange.password = userChange.password;
                                localStorage.setItem('accounts', JSON.stringify(accounts));
                                toast({ title: 'Success', message: 'Đổi mật khẩu thành công !', type: 'success', duration: 3000 });
                            } else {
                                document.querySelector('.password-after-comfirm-error').innerHTML = 'Mật khẩu bạn nhập không trùng khớp';
                            }
                        } else {
                            document.querySelector('.password-after-comfirm-error').innerHTML = 'Vui lòng xác nhận mật khẩu';
                        }
                    }
                } else {
                    document.querySelector('.password-after-info-error').innerHTML = 'Vui lòng nhập mật khẩu mới';
                }
            } else {
                document.querySelector('.password-cur-info-error').innerHTML = 'Bạn đã nhập sai mật khẩu hiện tại';
            }
        }
    }
}

function getProductInfo(id) {
    let products = JSON.parse(localStorage.getItem('products'));
    return products.find(item => {
        return item.id == id;
    })
}


// Format Date
function formatDate(date) {
    let fm = new Date(date);
    let yyyy = fm.getFullYear();
    let mm = fm.getMonth() + 1;
    let dd = fm.getDate();
    if (dd < 10) dd = '0' + dd;
    if (mm < 10) mm = '0' + mm;
    return dd + '/' + mm + '/' + yyyy;
}


// Back to top
window.onscroll = () => {
    let backtopTop = document.querySelector(".back-to-top")
    if (document.documentElement.scrollTop > 100) {
        backtopTop.classList.add("active");
    } else {
        backtopTop.classList.remove("active");
    }
}

// Auto hide header on scroll
//const headerNav = document.querySelector(".header-bottom");
//let lastScrollY = window.scrollY;

//window.addEventListener("scroll", () => {
//    if(lastScrollY < window.scrollY) {
//        headerNav.classList.add("hide")
//    } else {
//        headerNav.classList.remove("hide")
//    }
//    lastScrollY = window.scrollY;
//})

// Page
//function renderProducts(showProduct) {
//    let productHtml = '';
//    if(showProduct.length == 0) {
//        document.getElementById("home-title").style.display = "none";
//        productHtml = `<div class="no-result"><div class="no-result-h">Tìm kiếm không có kết quả</div><div class="no-result-p">Xin lỗi, chúng tôi không thể tìm được kết quả hợp với tìm kiếm của bạn</div><div class="no-result-i"><i class="fa-light fa-face-sad-cry"></i></div></div>`;
//    } else {
//        document.getElementById("home-title").style.display = "block";
//        showProduct.forEach((product) => {
//            productHtml += `<div class="col-product">
//            <article class="card-product" >
//                <div class="card-header">
//                    <a href="#" class="card-image-link" onclick="detailProduct(${product.id})">
//                    <img class="card-image" src="${product.img}" alt="${product.title}">
//                    </a>
//                </div>
//                <div class="food-info">
//                    <div class="card-content">
//                        <div class="card-title">
//                            <a href="#" class="card-title-link" onclick="detailProduct(${product.id})">${product.title}</a>
//                        </div>
//                    </div>
//                    <div class="card-footer">
//                        <div class="product-price">
//                            <span class="current-price">${vnd(product.price)}</span>
//                        </div>
//                    <div class="product-buy">
//                        <button onclick="detailProduct(${product.id})" class="card-button order-item"><i class="fa-regular fa-cart-shopping-fast"></i> Đặt món</button>
//                    </div> 
//                </div>
//                </div>
//            </article>
//        </div>`;
//        });
//    }
//    document.getElementById('home-products').innerHTML = productHtml;
//}

// Find Product
var productAll = JSON.parse(localStorage.getItem('products')).filter(item => item.status == 1);
//function searchProducts(mode) {
//    let valeSearchInput = document.querySelector('.form-search-input').value;
//    let valueCategory = document.getElementById("advanced-search-category-select").value;
//    let minPrice = document.getElementById("min-price").value;
//    let maxPrice = document.getElementById("max-price").value;
//    if(parseInt(minPrice) > parseInt(maxPrice) && minPrice != "" && maxPrice != "") {
//        alert("Giá đã nhập sai !");
//    }

//    let result = valueCategory == "Tất cả" ? productAll : productAll.filter((item) => {
//        return item.category == valueCategory;
//    });

//    result = valeSearchInput == "" ? result : result.filter(item => {
//        return item.title.toString().toUpperCase().includes(valeSearchInput.toString().toUpperCase());
//    })

//    if(minPrice == "" && maxPrice != "") {
//        result = result.filter((item) => item.price <= maxPrice);
//    } else if (minPrice != "" && maxPrice == "") {
//        result = result.filter((item) => item.price >= minPrice);
//    } else if(minPrice != "" && maxPrice != "") {
//        result = result.filter((item) => item.price <= maxPrice && item.price >= minPrice);
//    }

//    document.getElementById("home-service").scrollIntoView();
//    switch (mode){
//        case 0:
//            result = JSON.parse(localStorage.getItem('products'));;
//            document.querySelector('.form-search-input').value = "";
//            document.getElementById("advanced-search-category-select").value = "Tất cả";
//            document.getElementById("min-price").value = "";
//            document.getElementById("max-price").value = "";
//            break;
//        case 1:
//            result.sort((a,b) => a.price - b.price)
//            break;
//        case 2:
//            result.sort((a,b) => b.price - a.price)
//            break;
//    }
//    showHomeProduct(result)
//}

// Phân trang 
let perPage = 12;
let currentPage = 1;
let totalPage = 0;
let perProducts = [];

//function displayList(productAll, perPage, currentPage) {
//    let start = (currentPage - 1) * perPage;
//    let end = (currentPage - 1) * perPage + perPage;
//    let productShow = productAll.slice(start, end);
//    renderProducts(productShow);
//}

//function showHomeProduct(products) {
//    let productAll = products.filter(item => item.status == 1)
//    displayList(productAll, perPage, currentPage);
//    setupPagination(productAll, perPage, currentPage);
//}

//window.onload = showHomeProduct(JSON.parse(localStorage.getItem('products')))

function setupPagination(productAll, perPage) {
    document.querySelector('.page-nav-list').innerHTML = '';
    let page_count = Math.ceil(productAll.length / perPage);
    for (let i = 1; i <= page_count; i++) {
        let li = paginationChange(i, productAll, currentPage);
        document.querySelector('.page-nav-list').appendChild(li);
    }
}

function paginationChange(page, productAll, currentPage) {
    let node = document.createElement(`li`);
    node.classList.add('page-nav-item');
    node.innerHTML = `<a href="javascript:;">${page}</a>`;
    if (currentPage == page) node.classList.add('active');
    node.addEventListener('click', function () {
        currentPage = page;
        displayList(productAll, perPage, currentPage);
        let t = document.querySelectorAll('.page-nav-item.active');
        for (let i = 0; i < t.length; i++) {
            t[i].classList.remove('active');
        }
        node.classList.add('active');
        document.getElementById("home-service").scrollIntoView();
    })
    return node;
}

// Hiển thị chuyên mục
//function showCategory(category) {
//    document.getElementById('trangchu').classList.remove('hide');
//    document.getElementById('account-user').classList.remove('open');
//    document.getElementById('order-history').classList.remove('open');
//    let productSearch = productAll.filter(value => {
//        return value.category.toString().toUpperCase().includes(category.toUpperCase());
//    })
//    let currentPageSeach = 1;
//    displayList(productSearch, perPage, currentPageSeach);
//    setupPagination(productSearch, perPage, currentPageSeach);
//    document.getElementById("home-title").scrollIntoView();
//}


