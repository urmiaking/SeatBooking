# سیستم رزرو صندلی

این پروژه یک نمونه ساده از سیستم رزرو صندلی است که روی .NET 10 پیاده‌سازی شده و سه قابلیت اصلی را پوشش می‌دهد:

1) نمایش لحظه‌ای وضعیت صندلی‌ها (Available / Pending / Reserved)  
2) رزرو موقت (Hold) صندلی و پرداخت شبیه‌سازی‌شده  
3) مدیریت همزمانی درخواست‌ها (Concurrency) برای جلوگیری از رزرو همزمان یک صندلی

## سناریو و قوانین (مطابق تسک)

- هر صندلی در ابتدا Available است.
- وقتی یک کاربر روی صندلی Available رزرو را شروع کند:
  - صندلی به Pending تغییر می‌کند (رزرو موقت).
  - یک Reservation با وضعیت Pending ساخته می‌شود.
- کاربر تا قبل از پایان زمان رزرو می‌تواند پرداخت انجام دهد:
  - پرداخت موفق => صندلی Reserved می‌شود و رزرو Completed می‌شود.
  - پرداخت ناموفق => صندلی Pending باقی می‌ماند و کاربر می‌تواند مجدداً تلاش کند (تا قبل از انقضا).
- اگر تا پایان زمان رزرو (پیش‌فرض ۵ دقیقه) پرداخت کامل نشود:
  - رزرو Expired می‌شود.
  - صندلی دوباره Available می‌شود.
- تغییر وضعیت صندلی‌ها باید به صورت Real-time به کل کلاینت‌ها اعلام شود.

## معماری و لایه‌ها

پروژه به صورت چند لایه طراحی شده تا وابستگی‌ها کنترل‌شده باشند:

### 1) Domain
- شامل مدل‌های اصلی و قوانین بیزنسی:
  - Seat / SeatStatus
  - Reservation / ReservationStatus
- قوانین اصلی مثل:
  - تغییر وضعیت Seat (Pending / Reserved / Release)
  - Expire شدن Reservation

### 2) Application
- منطق کاربردی (Use Case) و سرویس‌ها:
  - SeatService (لیست صندلی‌ها)
  - ReservationService (شروع رزرو، پرداخت، انقضا، ریست)
  - PaymentService (شبیه‌سازی پرداخت با Delay و خروجی Success/Failed/Random)
- اعتبارسنجی‌ها (Validators) و DTO ها
- Abstractions (Interface ها) مثل:
  - IReservationService
  - ISeatService
  - IPaymentService
  - ISeatBookingNotifier (برای ارسال پیام‌های Real-time)

### 3) Infrastructure
- ذخیره‌سازی با EF Core (SQL Server)
- Repositoryها و Specificationها
- Migrationها
- نکته مهم: روی Seat از RowVersion برای optimistic concurrency استفاده شده است.

### 4) API
- کنترلرهای REST:
  - GET /api/seats
  - POST /api/reservations/start
  - POST /api/reservations/process-payment
  - POST /api/reservations/reset
- SignalR Hub و notifier:
  - Hub: /hubs/seat-booking
  - رویدادها:
    - SeatUpdated(seat)
    - SeatsUpdated(seats)

### 5) Web
- سرور اصلی برنامه (ASP.NET Core Razor Pages)
- صفحه Seats با Bootstrap و jQuery
- اتصال به API برای گرفتن لیست صندلی‌ها و انجام رزرو/پرداخت
- اتصال به SignalR برای آپدیت لحظه‌ای UI

## Real-time (SignalR)

SignalR برای اطلاع‌رسانی وضعیت صندلی‌ها به همه کلاینت‌ها استفاده می‌شود:

- وقتی یک صندلی Pending یا Reserved می‌شود: `SeatUpdated` ارسال می‌شود.
- وقتی چند صندلی با هم تغییر می‌کنند (مثل expire شدن گروهی یا reset): `SeatsUpdated` ارسال می‌شود.

در UI، رویداد `SeatsUpdated` به عنوان delta (لیست صندلی‌های تغییر کرده) merge می‌شود تا کل grid از بین نرود.

## نحوه انقضای رزروها (Background / Job)

انقضای رزروهای Pending با یک background service انجام می‌شود که به صورت دوره‌ای (پیش فرض 10 ثانیه):
- رزروهای Pending که از timeout گذشته‌اند را Expire می‌کند
- صندلی‌های مرتبط را Release می‌کند
- سپس با SignalR وضعیت صندلی‌های تغییر کرده را به کلاینت‌ها ارسال می‌کند

## مدیریت همزمانی (Concurrency) و RowVersion

برای جلوگیری از double-booking، روی جدول Seats از optimistic concurrency استفاده شده است:

- Seat دارای فیلد RowVersion است که در EF Core به صورت:
  - IsRowVersion()
  - IsConcurrencyToken()
  تنظیم شده.
- هنگام StartReservation چند کاربر ممکن است همزمان روی یک صندلی کلیک کنند؛ در این حالت فقط یکی از درخواست‌ها موفق می‌شود و باقی درخواست‌ها با Conflict (HTTP 409) برمی‌گردند.

### تست همزمانی (Concurrency Test)

برای اینکه مطمئن شویم سیستم جلوی رزرو همزمان یک صندلی را می‌گیرد، یک تست ساده از داخل DevTools مرورگر انجام شده است. ایده تست این است که برای یک `seatId` مشخص، ۲۰ درخواست همزمان به `POST /api/reservations/start` ارسال کنیم. انتظار داریم فقط یکی موفق شود و باقی درخواست‌ها با HTTP 409 برگردند.

### اجرای تست از داخل مرورگر

1) صفحه Seats را باز کنید.  
2) DevTools را باز کنید (Console).  
3) کد زیر را اجرا کنید:

```js
const seatId = (await $.getJSON("/api/seats"))[0].id;
const makeClientId = () => crypto.randomUUID();

const startReq = (clientId) =>
  $.ajax({
    method: "POST",
    url: "/api/reservations/start",
    contentType: "application/json",
    data: JSON.stringify({ seatId, clientId }),
  });

const tasks = Array.from({ length: 20 }, () => startReq(makeClientId()));
const results = await Promise.allSettled(tasks);
results.map(r => (r.status === "fulfilled" ? "OK" : r.reason?.status));
```

### نتیجه تست انجام‌شده

خروجی تست ۲۰ درخواست همزمان برای یک صندلی:

(20) ['OK', 409, 409, 409, 409, 409, 409, 409, 409, 409, 409, 409, 409, 409, 409, 409, 409, 409, 409, 409]

این نتیجه نشان می‌دهد که فقط یک درخواست توانسته صندلی را Pending کند و باقی درخواست‌ها به دلیل برخورد همزمانی با Conflict رد شده‌اند. این رفتار از طریق optimistic concurrency و فیلد RowVersion روی Seat تضمین می‌شود.

## راه‌اندازی پروژه

### پیش‌نیازها
- .NET SDK 10
- Visual Studio 2026 
- SQL Server LocalDB (پیش‌فرض در appsettings) یا SQL Server

### تنظیمات دیتابیس
Connection string پیش‌فرض داخل:
- src/SeatBooking.Web/appsettings.json

### اجرا
1) Solution را باز کنید.
2) پروژه Startup را SeatBooking.Web قرار دهید.
3) Run کنید.

در شروع برنامه:
- Migrationها اعمال می‌شوند.
- Seed دیتابیس اجرا می‌شود (ایجاد صندلی‌ها).

### آدرس‌های مهم
- UI: /Seats
- Swagger (در Development): /swagger
- SignalR Hub: /hubs/seat-booking

## API ها (خلاصه)

- GET /api/seats  
  خروجی: لیست صندلی‌ها

- POST /api/reservations/start  
  body:
  - seatId (Guid)
  - clientId (Guid)  
  خروجی: reservationId

- POST /api/reservations/process-payment  
  body:
  - reservationId (Guid)
  - clientId (Guid)
  - paymentOutcome: Success / Failed / Random  
  خروجی: PaymentStatus (Succeeded/Failed)

- POST /api/reservations/reset  
  تمام رزروها حذف می‌شوند و همه صندلی‌ها Available می‌شوند (برای تست و دمو).

## نکات اجرایی و محدودیت‌ها

- تمرکز پروژه روی correctness، همزمانی و real-time update است.
- Authentication واقعی پیاده‌سازی نشده و clientId نقش شناسه کاربر را دارد.
- Timeout و MaxSeats در SeatBookingSettings قرار دارد و می توان در AppSettings مقدار اولیه به آن داد.
