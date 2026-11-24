# PROJECT CONTEXT: ArtShop (tên tạm)

> File này mô tả **mục tiêu, nghiệp vụ và quyết định thiết kế** của dự án.
> Mọi agent/lập trình viên cần đọc file này **trước khi viết code**.
> Khi có quyết định mới ảnh hưởng đến nghiệp vụ hoặc phạm vi, hãy cập nhật file này.

---

## 1. Tóm tắt

Một website **bán tranh vật lý của một họa sĩ** (gallery 1 tác giả), xây bằng **ASP.NET Core + React**.

**Mục đích thật của dự án: project portfolio để bỏ vào CV.** Đây không phải sản phẩm thương mại. Vì vậy mọi quyết định cần cân nhắc theo hai tiêu chí:

1. **Chạy được thật** trên hạ tầng **miễn phí**, có link demo công khai.
2. **Có điểm nhấn kỹ thuật** có thể kể trong phỏng vấn, thay vì nhiều tính năng nông.

> Lịch sử: ban đầu dự án hướng đến việc đóng gói để bán source cho họa sĩ. Hướng đó đã **bỏ**. Một số thiết kế vẫn được giữ lại vì chúng cũng là điểm cộng kỹ thuật (xem mục 5).

## 2. Người dùng trong hệ thống

| Vai trò | Mô tả |
|---|---|
| **Buyer** (Customer) | Khách xem, tìm, mua tranh trên website |
| **Admin** (họa sĩ/chủ shop) | Đăng tranh, quản lý đơn hàng, chỉnh thông tin shop |

Không có vai trò Seller. Không có marketplace.

## 3. Phạm vi

### Trong phạm vi
- Bán **tranh vật lý** (bản gốc, mỗi tác phẩm thường là duy nhất).
- Storefront cho Buyer và trang quản trị cho Admin.
- Thanh toán đơn giản: **chuyển khoản (VietQR) và COD**.
- Chạy local bằng Docker Compose, deploy demo trên hạ tầng free.

### Ngoài phạm vi (non-goals), không tự ý làm
- Marketplace nhiều người bán, hoa hồng, chia tiền, escrow.
- Multi-tenant / SaaS.
- Bán source, license key, setup wizard cho nhiều khách.
- Tích hợp cổng thanh toán thật (VNPay/MoMo/Stripe) vì cần hồ sơ merchant. Chỉ giữ interface để chứng minh thiết kế mở rộng được; có thể làm provider giả lập (sandbox) nếu muốn demo.
- Đấu giá, AR, NFT, tranh số.

## 4. Mục tiêu "điểm nhấn kỹ thuật" (ưu tiên làm kỹ)

Đây là những phần cần làm chắc tay vì có giá trị khi trình bày trong CV/phỏng vấn:

1. **Xử lý tranh duy nhất (tồn kho = 1) và đồng thời (concurrency).** `RowVersion`, trạng thái `Reserved`, `BackgroundService` trả tranh về `Available` khi hết hạn giữ hàng.
2. **Xác thực và phân quyền:** ASP.NET Core Identity + JWT (hoặc cookie HttpOnly), role `Admin`/`Customer`, policy.
3. **Kiến trúc sạch:** logic nghiệp vụ ở service layer, validation (FluentValidation), xử lý lỗi thống nhất, log (Serilog).
4. **Trừu tượng hóa hạ tầng:** `IImageStorage`, `IPaymentProvider` (dependency injection, dễ thay implementation).
5. **Upload và xử lý ảnh an toàn:** decode lại, xóa EXIF, nhiều kích thước, WebP.
6. **DevOps cơ bản:** Docker Compose chạy bằng một lệnh, biến môi trường cho secret, CI (GitHub Actions: build + test).
7. **Test:** unit test cho service đặt hàng và giữ hàng; ít nhất một integration test.
8. **Frontend:** React + TypeScript, TanStack Query, responsive, ảnh tối ưu (`srcSet`, lazy loading).

## 5. Nguyên tắc thiết kế

1. **Config thay vì hardcode (giữ ở mức nhẹ).** Tên shop, họa sĩ, logo, màu chủ đạo, phí ship, chính sách đổi trả, thông tin chuyển khoản lưu trong bảng `SiteSettings` (một dòng) và sửa được trong admin. Giữ lại vì dễ làm và cho thấy tư duy tách cấu hình khỏi code.
2. **Logic nghiệp vụ nằm ở service**, controller chỉ điều phối.
3. **Không tin dữ liệu từ client.** Giá và tổng tiền luôn tính lại ở server.
4. **Secret không commit lên Git.** Dùng biến môi trường; có `.env.example`.
5. **Giữ phạm vi nhỏ.** Làm ít tính năng nhưng chạy mượt và có test, hơn là nhiều tính năng dở dang.
6. **Ngôn ngữ.** Code bằng tiếng Anh

## 6. Quy tắc nghiệp vụ cốt lõi (bán tranh vật lý)

- **Tồn kho = 1.** Hai người không thể cùng mua một tranh.
- **Giữ hàng (reservation).** Khi khách bắt đầu thanh toán, tranh chuyển `Reserved` trong thời hạn cấu hình (mặc định 15-30 phút). Hết hạn mà chưa thanh toán thì tự về `Available`.
- **Chống mua trùng.** `RowVersion` trên `Painting`. Gặp `DbUpdateConcurrencyException` thì báo "tranh vừa được người khác đặt".
- **Lưu giá lúc mua.** `OrderItem.PriceAtPurchase` bất biến, không đọc lại giá hiện tại.
- **Thuộc tính tranh:** chất liệu, kích thước (cm), năm sáng tác, có khung hay chưa, mô tả, nhiều ảnh.
- **Vận chuyển:** MVP dùng phí cố định lấy từ `SiteSettings`.
- Admin phải đặt thủ công được trạng thái `Sold`/`Hidden` (ví dụ tranh bán ngoài web).

### Vòng đời trạng thái

- **Painting.Status:** `Available` → `Reserved` → `Sold`; về lại `Available` nếu hết hạn hoặc hủy; `Hidden` do admin ẩn.
- **Order.Status:** `Pending` → `Paid` → `Processing` → `Shipped` → `Completed`; nhánh `Cancelled`, `Refunded`.
- **Payment.Status:** `Unpaid` → `Paid` | `Failed` | `Refunded`.

> Đơn bị hủy hoặc hết hạn thanh toán thì tranh trong đơn phải trả về `Available`.

## 7. Công nghệ

| Tầng | Lựa chọn |
|---|---|
| Backend | ASP.NET Core Web API (bản LTS mới nhất), EF Core |
| Database | **PostgreSQL** (Npgsql) |
| Xác thực | ASP.NET Core Identity + JWT (hoặc cookie HttpOnly) |
| Frontend | React + TypeScript + Vite, React Router, TanStack Query, Axios |
| UI | Tailwind CSS |
| Ảnh | `IImageStorage`: Local (dev), Cloudinary (demo) |
| Khác | Serilog, FluentValidation, xUnit, Docker Compose, GitHub Actions |

SEO của SPA (Vite) yếu. Với portfolio thì chấp nhận được; có thể nhắc trong README như hạn chế đã biết và hướng cải thiện (SSR/prerender).

## 8. Hạ tầng miễn phí cho bản demo

Giới hạn của gói free **thay đổi thường xuyên**, luôn kiểm tra lại trang chính thức trước khi dùng.

| Thành phần | Gợi ý | Lưu ý |
|---|---|---|
| Database | **Neon** (Postgres free) | Tự ngủ khi rảnh, request đầu có thể chậm; dung lượng free có giới hạn |
| API (.NET) | **Render** (Docker, free) | Dịch vụ free tự ngủ khi không có truy cập, lần mở đầu có thể mất 30-60 giây |
| Frontend (React) | Vercel / Netlify / Cloudflare Pages | Build ra file tĩnh, host free |
| Ảnh | **Cloudinary** (gói free) | Host free thường **không giữ file trên ổ đĩa lâu dài**, nên không dùng Local storage cho bản demo |

Hệ quả cho code:

- Chuỗi kết nối và secret đọc từ **biến môi trường**.
- Neon cần SSL: thêm `SSL Mode=Require` vào chuỗi kết nối.
- Bật `EnableRetryOnFailure()` cho EF Core để chịu được lúc database vừa thức dậy.
- Chọn implementation ảnh bằng cấu hình `ImageStorage:Provider` (`Local` hoặc `Cloudinary`).
- README phải ghi rõ: lần mở đầu có thể chậm vì dịch vụ free tự ngủ.

Khi dev local: dùng container Postgres trong `docker-compose.yml`, không cần Neon.

## 9. Mô hình dữ liệu (tóm tắt)

- `SiteSettings` (một dòng, Id = 1): ShopName, ArtistName, ArtistBio, ArtistPhotoUrl, LogoUrl, PrimaryColor, ContactEmail, BankAccountInfo, ReturnPolicy, FlatShippingFee, ReservationMinutes.
- `Painting`: Id, Title, Description, Price, WidthCm, HeightCm, Medium, Year, IsFramed, Status, CategoryId (hoặc Collection), RowVersion.
- `PaintingImage`: Id, PaintingId, StorageKey, ThumbUrl, MediumUrl, FullUrl, Width, Height, SortOrder.
- `Category` / `Collection`.
- `Cart` / `CartItem`.
- `Order`: Id, UserId, Total, Status, PaymentStatus, ShippingAddress, thông tin liên hệ, ReservedUntil.
- `OrderItem`: OrderId, PaintingId, PriceAtPurchase.
- `User`: từ Identity, role `Admin` và `Customer`.

Không có bảng `Artist` dạng danh sách (chỉ một họa sĩ, thông tin nằm trong `SiteSettings`), không có `SellerId`, không có `CommissionRate`.

## 10. Tính năng

### MVP (bắt buộc)
**Storefront:**
- Trang chủ, danh sách tranh, lọc (chất liệu, giá, kích thước), tìm kiếm, phân trang
- Chi tiết tranh: gallery ảnh, zoom
- Trang giới thiệu họa sĩ (lấy từ `SiteSettings`)
- Đăng ký/đăng nhập, giỏ hàng, checkout, xem lịch sử đơn

**Admin:**
- CRUD tranh, upload nhiều ảnh, sắp xếp ảnh
- Quản lý đơn hàng, đổi trạng thái, xác nhận đã nhận chuyển khoản
- Trang chỉnh `SiteSettings`

**Hạ tầng:** Docker Compose, tự chạy migration, seed dữ liệu mẫu (có tranh mẫu đẹp và tài khoản admin demo).

### Nên có nếu còn thời gian
- Giữ hàng có hết hạn (`BackgroundService`), nếu chưa làm trong MVP
- Email xác nhận đơn
- Wishlist, bộ sưu tập nổi bật
- Unit/integration test, GitHub Actions
- Mã QR chuyển khoản (VietQR) trong trang thanh toán

### Chưa làm
Mọi thứ trong mục 3 "Ngoài phạm vi".

## 11. Thanh toán

```csharp
public interface IPaymentProvider
{
    string Code { get; }                      // "bank", "cod"
    Task<PaymentInit> CreateAsync(Order order);
    Task<bool> VerifyCallbackAsync(HttpRequest request);
}
```

- **Bank (chuyển khoản + VietQR):** hiển thị thông tin/QR, admin xác nhận thủ công.
- **COD:** thanh toán khi nhận hàng.
- Cổng thật (VNPay/MoMo/Stripe) **không làm**. Interface vẫn được giữ để chứng minh khả năng mở rộng.

## 12. Cấu trúc thư mục dự kiến

```
ArtShop/
├── PROJECT_CONTEXT.md
├── README.md
├── docker-compose.yml
├── .env.example
├── ArtShop.Api/
│   ├── Controllers/
│   ├── Services/         # logic nghiệp vụ
│   ├── Payments/         # IPaymentProvider + bank, cod
│   ├── Storage/          # IImageStorage + Local, Cloudinary
│   ├── Data/             # DbContext, entity, migration, seed
│   └── Program.cs
├── ArtShop.Tests/        # xUnit
└── artshop-web/
    └── src/
        ├── api/
        ├── pages/        # storefront + admin
        ├── components/
        └── SettingsProvider.tsx
```

## 13. Bảo mật tối thiểu

- Chỉ Admin được thao tác tranh, ảnh, đơn của người khác (`[Authorize(Roles = "Admin")]`).
- Upload ảnh: giới hạn dung lượng và số lượng, chỉ nhận jpg/png/webp, **decode lại ảnh**, không dùng tên file của client, xóa EXIF.
- Cấu hình CORS đúng origin của frontend (cả local lẫn bản demo).
- Không commit secret; mật khẩu admin demo chỉ dùng cho dữ liệu mẫu.
- Mật khẩu người dùng do Identity băm, không tự viết.

## 14. Chuẩn bị để bỏ vào CV

- **Link demo** chạy được, có dữ liệu mẫu, có tài khoản admin demo (ghi trong README).
- **README** có: mô tả ngắn, ảnh chụp màn hình, sơ đồ kiến trúc/ERD, cách chạy local (một lệnh), danh sách điểm nhấn kỹ thuật (mục 4), hạn chế đã biết.
- Một đoạn mô tả ngắn cho CV, ví dụ: *"Sàn bán tranh vật lý (ASP.NET Core, React, PostgreSQL): xử lý đặt hàng đồng thời cho sản phẩm duy nhất bằng optimistic concurrency và giữ hàng có thời hạn; JWT + phân quyền; upload ảnh đa kích thước; Docker Compose; CI."* Chỉ ghi những gì **đã làm thật**.
- Chuẩn bị trả lời: vì sao chọn `RowVersion` thay vì lock; điều gì xảy ra khi hai người thanh toán cùng lúc; vì sao tách `IImageStorage`; vì sao tính giá ở server.

## 15. Quy ước cho agent khi làm việc

- Đọc file này trước và **không tự thêm tính năng ngoài phạm vi** (mục 3).
- Ưu tiên chất lượng các điểm nhấn ở mục 4 hơn là thêm tính năng mới.
- Không hardcode thông tin thương hiệu; dùng `SiteSettings`.
- Mọi thay đổi trạng thái tranh/đơn phải đi qua service và tôn trọng mục 6.
- Tính tiền ở server; kiểm tra quyền bằng role/policy.
- Thêm nơi lưu ảnh hoặc cổng thanh toán bằng cách implement interface, không sửa luồng chính.
- Mỗi thay đổi schema phải có migration.
- Viết test cho logic đặt hàng/giữ hàng khi sửa phần này.
- Không để secret hay chuỗi kết nối thật trong code hoặc commit.

## 16. Lộ trình gợi ý

1. **Tuần 1:** solution, Postgres (Docker), migration, seed, API đọc tranh.
2. **Tuần 2:** frontend: trang chủ, danh sách, chi tiết, bộ lọc.
3. **Tuần 3:** đăng ký/đăng nhập (JWT), giỏ hàng.
4. **Tuần 4:** checkout, đơn hàng, giữ hàng với `RowVersion` + `BackgroundService`.
5. **Tuần 5:** admin: CRUD tranh, upload ảnh (Cloudinary), quản lý đơn, `SiteSettings`.
6. **Tuần 6:** test, GitHub Actions, Dockerfile, deploy (Neon + Render + Vercel/Netlify).
7. **Tuần 7:** hoàn thiện README, ảnh chụp màn hình, dữ liệu demo, kiểm tra link demo từ máy khác.

## 17. Thuật ngữ

| Thuật ngữ | Nghĩa |
|---|---|
| Buyer / Customer | Người mua tranh trên website |
| Admin | Họa sĩ/chủ shop quản lý nội dung và đơn hàng |
| Reserved | Tranh đang được giữ cho một đơn chưa thanh toán xong |
| `RowVersion` | Cột phiên bản để EF Core phát hiện xung đột cập nhật (optimistic concurrency) |
| `SiteSettings` | Bản ghi cấu hình duy nhất của shop |
| COD | Cash on Delivery, thanh toán khi nhận hàng |
