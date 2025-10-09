import { initializeApp } from "https://www.gstatic.com/firebasejs/12.3.0/firebase-app.js";
import { getMessaging, getToken, onMessage, deleteToken } from "https://www.gstatic.com/firebasejs/12.3.0/firebase-messaging.js";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

const firebaseConfig = {
    apiKey: "AIzaSyBWkAx2RElXJjHJIFHoO3uOp67tCrAgYpo",
    authDomain: "goimon-ce886.firebaseapp.com",
    projectId: "goimon-ce886",
    storageBucket: "goimon-ce886.firebasestorage.app",
    messagingSenderId: "429500775117",
    appId: "1:429500775117:web:2ef2f4d48686812fadc784",
    measurementId: "G-VHLVQYZBCC"
};

const app = initializeApp(firebaseConfig);
const messaging = getMessaging(app);

// dành cho apple
document.addEventListener("DOMContentLoaded", function () {
    const btn = document.getElementById("btnEnableNotificationOfOrder");

    // Nếu không có nút thì dừng
    if (!btn) return;

    // Kiểm tra quyền thông báo
    //if (Notification.permission === "granted") {
    //    // Đã bật thông báo => ẩn nút
    //    btn.style.display = "none";
    //} else {
    //    // Chưa bật => hiện nút
    //    btn.style.display = "inline-block";
    //}

    // Lưu lại HTML gốc của nút
    const originalHTML = btn.innerHTML;

    // Gắn sự kiện click
    btn.addEventListener("click", async () => {
        // Hiện biểu tượng loading
        btn.innerHTML = `<i class="fa-solid fa-loader fa-spin fa-spin-reverse"></i>`;
        btn.disabled = true; // tạm khóa nút

        try {
            // Hỏi người dùng bằng confirm
            const confirmEnable = confirm("Bạn có muốn bật thông báo không?");
            if (!confirmEnable) {
                //alert("Bạn đã hủy yêu cầu bật thông báo.");
                return;
            }

            // Xin quyền thông báo
            const permission = await Notification.requestPermission();

            if (permission === "granted") {
                await resetAndRegisterToken();
                btn.disabled = true; // tạm khóa nút
                //new Notification("✅ Bạn đã bật thông báo thành công!");
                // Ẩn nút khi bật xong
                //btn.style.display = "none";
            } else {
                //alert("❌ Bạn đã từ chối thông báo.");
            }
        } catch (err) {
            console.error("Lỗi khi bật thông báo:", err);
            toast({ title: 'Lỗi', message: 'Đã xảy ra lỗi khi bật thông báo.' || 'Xảy ra lỗi', type: 'error', duration: 5000 });
        } finally {
            // Khôi phục lại nút
            btn.innerHTML = originalHTML;
            btn.disabled = false;
        }
    });
});
async function resetAndRegisterToken() {
    try {
        // Xoá token cũ (await để đảm bảo thực hiện xong)
        const deleted = await deleteToken(messaging);
        //console.log("deleteToken result:", deleted); // boolean: true nếu đã xóa, false nếu không có token

        // Lấy token mới (chắc chắn chạy sau khi deleteToken hoàn tất)
        const currentToken = await getToken(messaging, {
            vapidKey: "BFmgu7QyM9Cv7M-pnD3xHE71DquQeJhBhGed1qdN0fJIaYc7-YqOHA_C3mtS2icfxQNe6Xp6VAaqezJoTEvSYI4"
        });

        if (!currentToken) {
            console.warn("Không lấy được token mới");
            return;
        }
        else {
            //console.log("currentToken:", currentToken);
        }

        // Gửi token lên server và chờ server lưu xong
        const res = await fetch("/api/firebase/register-topic", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ token: currentToken })
        });

        if (!res.ok) {
            const text = await res.text();
            throw new Error("Lỗi lưu token trên server: " + text);
        }

        const data = await res.json();
        //console.log("Token sent successfully:", data);
        playNotificationSound();
        toast({ title: 'Hệ thống sẵn sàng!', message: 'Chúc bạn làm việc vui vẽ 🥰🎉', type: 'success', duration: 3000 });

    } catch (err) {
        console.error("Lỗi khi reset/register token:", err);
        toast({ title: 'Lỗi', message: 'Chưa đăng ký được thông báo đơn hàng' || 'Xảy ra lỗi', type: 'error', duration: 5000 });
    }
}

// dành cho hệ sinh thái google
// Xin quyền nhận notification
//Notification.requestPermission().then((permission) => {
//    if (permission === "granted" && shareData.currentController == 'AdminHome') {

//        async function resetAndRegisterToken() {
//            try {
//                // Xoá token cũ (await để đảm bảo thực hiện xong)
//                const deleted = await deleteToken(messaging);
//                //console.log("deleteToken result:", deleted); // boolean: true nếu đã xóa, false nếu không có token

//                // Lấy token mới (chắc chắn chạy sau khi deleteToken hoàn tất)
//                const currentToken = await getToken(messaging, {
//                    vapidKey: "BFmgu7QyM9Cv7M-pnD3xHE71DquQeJhBhGed1qdN0fJIaYc7-YqOHA_C3mtS2icfxQNe6Xp6VAaqezJoTEvSYI4"
//                });

//                if (!currentToken) {
//                    console.warn("Không lấy được token mới");
//                    return;
//                }
//                else {
//                    //console.log("currentToken:", currentToken);
//                }

//                // Gửi token lên server và chờ server lưu xong
//                const res = await fetch("/api/firebase/register-topic", {
//                    method: "POST",
//                    headers: { "Content-Type": "application/json" },
//                    body: JSON.stringify({ token: currentToken })
//                });

//                if (!res.ok) {
//                    const text = await res.text();
//                    throw new Error("Lỗi lưu token trên server: " + text);
//                }

//                const data = await res.json();
//                //console.log("Token sent successfully:", data);
//                toast({ title: 'Hệ thống sẵn sàng!', message: 'Chúc bạn làm việc vui vẽ 🥰🎉', type: 'success', duration: 3000 });

//            } catch (err) {
//                console.error("Lỗi khi reset/register token:", err);
//                toast({ title: 'Lỗi', message: 'Chưa đăng ký được thông báo đơn hàng' || 'Xảy ra lỗi', type: 'error', duration: 5000 });
//            }
//        }

//        // Gọi hàm
//        resetAndRegisterToken();
//    }
//});
function isDesktopDevice() {
    // Kiểm tra bằng userAgent
    return !/Mobi|Android|iPhone|iPad|iPod/i.test(navigator.userAgent);
}
function playNotificationSound() {
    if (!isDesktopDevice()) return; // Chỉ phát nếu là máy tính
    const audio = new Audio("/Content/sounds/notify.wav");
    audio.volume = 0.5; // âm lượng 
    audio.play().catch(err => {
        console.warn("Không phát được âm thanh:", err);
    });
}
// Nhận thông báo khi web đang mở (foreground)
onMessage(messaging, (payload) => {
    shareData.countNotification += 1;
    //console.log("Tin nhắn foreground:", payload);
    //alert(payload.notification.title + " - " + payload.notification.body);
    playNotificationSound();
    toast({ title: payload.notification.title, message: payload.notification.body, type: 'success', duration: 3000 });
});