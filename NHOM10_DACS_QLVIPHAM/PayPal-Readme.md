# Hướng dẫn cài đặt PayPal cho hệ thống thanh toán

## Bước 1: Tạo tài khoản PayPal Developer

1. Truy cập trang [PayPal Developer](https://developer.paypal.com/) và đăng ký/đăng nhập tài khoản
2. Di chuyển đến Dashboard

## Bước 2: Tạo ứng dụng để lấy thông tin API 

1. Vào mục "My Apps & Credentials"
2. Chọn tab "REST API apps"
3. Nhấn vào "Create App"
4. Điền tên ứng dụng và chọn "Sandbox" cho môi trường thử nghiệm
5. Nhấn "Create App"

## Bước 3: Lấy thông tin API Credentials

1. Sau khi tạo, bạn sẽ thấy thông tin Client ID và Secret
2. Lưu lại hai thông tin này

## Bước 4: Cập nhật cấu hình trong hệ thống

1. Mở file `appsettings.json`
2. Tìm phần cấu hình PayPal:
   ```json
   "PayPal": {
     "ClientId": "YOUR_PAYPAL_CLIENT_ID",
     "ClientSecret": "YOUR_PAYPAL_CLIENT_SECRET",
     "Mode": "sandbox"
   }
   ```
3. Thay thế `YOUR_PAYPAL_CLIENT_ID` và `YOUR_PAYPAL_CLIENT_SECRET` bằng thông tin API đã lấy
4. Lưu file

## Bước 5: Tạo tài khoản Sandbox để kiểm thử

1. Vào mục "Sandbox" > "Accounts"
2. Tài khoản Personal và Business đã được tạo sẵn
3. Lưu lại email và mật khẩu của tài khoản Personal để dùng cho việc test

## Lưu ý quan trọng khi triển khai thực tế

1. Khi triển khai cho môi trường thực tế (Production), thay đổi `Mode` thành `"live"` trong `appsettings.json`
2. Tạo ứng dụng mới trong môi trường Live và cập nhật Client ID và Secret mới
3. Đảm bảo lưu trữ thông tin API một cách an toàn
4. Kiểm tra kỹ các giao dịch trong môi trường thử nghiệm trước khi chuyển sang production 