# Parcel Scan Event Processing Solution (.NET 8)

This solution simulates a simplified event-driven system for tracking parcel scan events. It consists of **three projects**, each serving a clear responsibility:

---

## 📦 Projects Overview

### 1. **ScanEventAPI**

- A mock **RESTful API** that exposes parcel scan events.
- Endpoint: `GET /api/scan-events?eventId={lastEventId}&limit={batchSize}`
- Returns a list of scan events (e.g., `PICKUP`, `DELIVERY`, `STATUS`) starting from a specific event ID.
- Useful for simulating real-time scan activity in an event source system.

---

### 2. **ScanWorkerAPI**

- A **.NET Worker Service** (`BackgroundService`) that:
  - Periodically **polls** the `ScanEventAPI` for new scan events.
  - Uses **Refit** to simplify the API interaction.
  - Processes each event using a **BlockingCollection** (producer-consumer pattern).
  - Publishes each processed event (currently logs to console).

#### 🔄 Refit vs HttpClient

| Feature         | Refit                                      | HttpClient               |
|----------------|---------------------------------------------|--------------------------|
| Simplicity      | ✅ Declarative, interface-driven              | ❌ Manual request setup  |
| Code Reuse      | ✅ Cleaner service abstractions              | ❌ Verbose boilerplate   |
| Maintainability | ✅ Easier to refactor and test              | ❌ Harder to mock/test   |

Refit enables concise and type-safe HTTP clients via interfaces, reducing boilerplate and improving maintainability.

---

### 3. **Data Persistence**

- The project uses **SQLite** as a lightweight embedded database.
- The database file is **automatically created** on first run (`App_Data/scanworker.db`).
- It stores:
  - `LastProcessedEvent`: to persist the last consumed `EventId`, ensuring resume support after restart.
  - `PlateInformation`: representing contextual data linked with scan events, saved **before publishing**.

---

## ⚙️ Architecture Highlights

### ✅ BlockingCollection with Producer-Consumer Pattern

- **Producer**: `PollScanEventsAsync()` continuously polls `ScanEventAPI` and adds events to the queue.
- **Consumer**: `ConsumeAndProcessEventsAsync()` dequeues and processes each event, ensuring separation of concerns and backpressure handling.

---

### ✅ Logging

- Integrated with **Serilog**:
  - Logs to **console** and to **rolling files** (`logs/log-<date>.txt`)
  - Configurable via `loggersettings.json`.

---

### ✅ Event Publishing

- Each event is processed and then passed to a **publisher** component.
- Currently implemented as a **console-based publisher** (`ConsolePublisher`) for demo purposes.
- Easily extensible for Kafka, RabbitMQ, Azure Service Bus, etc.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio or VS Code

### Run the Solution

1. **Start the ScanEventAPI** (provides the event data).
2. **Run the ScanWorkerAPI** (will poll and consume the events).
3. View logs in:
   - Console output
   - Log files in `/logs` folder

SQLite DB will be automatically created at:
