// firebase-messaging-sw.js
importScripts("https://www.gstatic.com/firebasejs/12.3.0/firebase-app-compat.js");
importScripts("https://www.gstatic.com/firebasejs/12.3.0/firebase-messaging-compat.js");


firebase.initializeApp({
    apiKey: "AIzaSyBWkAx2RElXJjHJIFHoO3uOp67tCrAgYpo",
    authDomain: "goimon-ce886.firebaseapp.com",
    projectId: "goimon-ce886",
    storageBucket: "goimon-ce886.firebasestorage.app",
    messagingSenderId: "429500775117",
    appId: "1:429500775117:web:2ef2f4d48686812fadc784",
    measurementId: "G-VHLVQYZBCC"
});

const messaging = firebase.messaging();

//let oldMessageNotification = '';
//// Nhận thông báo khi web tắt hoặc chạy background
//messaging.onBackgroundMessage((payload) => {
//    console.log("Background message received:", payload);
//    const notificationTitle = payload.notification.title;
//    const notificationOptions = {
//        body: payload.notification.body,
//        icon: "/assets/img/order.png"
//    };
//    if (oldMessageNotification != payload.notification.body) {
//        //self.registration.showNotification(notificationTitle, notificationOptions);
//        oldMessageNotification = payload.notification.body;
//    }
//});