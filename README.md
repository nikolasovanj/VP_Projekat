# Battery EIS Measurement Transfer System

A client-server application for transferring, validating, processing, and storing **Electrochemical Impedance Spectroscopy (EIS)** measurements from battery datasets.

The project uses **WCF over Net.TCP** for communication between a client and a battery measurement service. The client selects a battery dataset and streams its measurements to the service, while the service validates and processes each sample, detects temperature spikes, separates accepted and rejected measurements, and stores the results as CSV files.

---

## Overview

The system is divided into three projects:

```text
VP_Projekat
│
├── Client
│   └── Selects a dataset and sends EIS samples
│
├── Service
│   └── Receives, validates and stores samples
│
├── Common
│   └── Shared models, WCF contract and processing utilities
│
└── Dataset
    └── Battery EIS measurement data
```

The basic workflow is:

```text
                 ┌──────────────────┐
                 │     Dataset      │
                 │                  │
                 │ Battery / Test / │
                 │       SoC        │
                 └────────┬─────────┘
                          │
                          ▼
                 ┌──────────────────┐
                 │      Client      │
                 │                  │
                 │ Select metadata  │
                 │ Read CSV samples │
                 └────────┬─────────┘
                          │
                    WCF / Net.TCP
                    localhost:9000
                          │
                          ▼
                 ┌──────────────────┐
                 │     Service      │
                 │                  │
                 │ Validate sample  │
                 │ Check temperature│
                 │ Check range      │
                 │ Write results    │
                 └────────┬─────────┘
                          │
                ┌─────────┴─────────┐
                ▼                   ▼
        ┌──────────────┐    ┌──────────────┐
        │ session.csv  │    │ reject.csv   │
        │ Valid data   │    │ Rejected data│
        └──────────────┘    └──────────────┘
```

---

## Functionality

### Dataset Selection

When the client starts a transfer, it automatically selects a random dataset from the available battery measurements.

The metadata contains:

* Battery ID
* Test ID
* State of Charge (SoC)
* Source file name
* Number of rows

The dataset is selected from the following structure:

```text
Dataset/
└── B01/
    └── EIS measurements/
        ├── Test_1/
        │   └── Hioki/
        │       └── measurement.csv
        │
        └── Test_2/
            └── Hioki/
                └── measurement.csv
```

The repository contains datasets for:

```text
B01
B02
B03
B04
B05
B06
B07
B08
B09
B10
B11
```

with both `Test_1` and `Test_2` measurements.

The included dataset contains **440 CSV measurement files**.

---

## Client

The client is a console application responsible for selecting and sending measurement data.

When started, it presents a simple menu:

```text
[ANY]. Send samples
2. Exit
Choose action:
```

Any input other than `2` starts a measurement transfer.

### Transfer process

The client:

1. Randomly selects battery metadata.
2. Creates an `EisMeta` object.
3. Opens the selected CSV file.
4. Calls `StartSession()` on the service.
5. Reads the measurement rows one by one.
6. Converts every row into an `EisSample`.
7. Sends each sample through WCF.
8. Calls `EndSession()` when all samples have been transferred.

Conceptually:

```text
EisMeta
   │
   ▼
SampleReader
   │
   ├── Read CSV row
   │
   ▼
EisSample
   │
   ▼
IBattery.PushSample()
   │
   ▼
BatteryService
```

---

## WCF Service

The service exposes the `IBattery` contract over **Net.TCP**.

The service listens on:

```text
net.tcp://localhost:9000/BatteryService
```

The WCF contract is:

```csharp
public interface IBattery
{
    string StartSession(EisMeta eisMeta);

    void PushSample(EisSample eisSample);

    void EndSession(string path);
}
```

### `StartSession`

Starts a new measurement transfer.

The service:

* Creates the output directory structure.
* Creates `session.csv`.
* Creates `reject.csv`.
* Registers the session.
* Raises a transfer-start event.
* Returns the session path to the client.

Output directories follow:

```text
Data/
└── <BatteryId>/
    └── <TestId>/
        └── <SoC>%
            ├── session.csv
            └── reject.csv
```

For example:

```text
Data/
└── B01/
    └── Test_1/
        └── 50%/
            ├── session.csv
            └── reject.csv
```

---

### `PushSample`

`PushSample()` is called for every measurement received from the client.

For every sample, the service:

1. Raises a sample-received event.
2. Checks the temperature against the previous measurement.
3. Detects significant temperature changes.
4. Determines whether the sample passes the configured range check.
5. Writes the sample to either `session.csv` or `reject.csv`.
6. Updates the previous temperature.

The service intentionally introduces a `500 ms` delay between samples.

---

### `EndSession`

When all samples have been transmitted, the client calls:

```csharp
EndSession(path);
```

The service closes the corresponding output files and raises a transfer-completed event.

---

# EIS Sample Format

Each input CSV row contains six measurement values:

```text
FrequencyHz,R_ohm,X_ohm,V,T_degC,Range_ohm
```

These are converted into an `EisSample` containing:

| Property         | Description                            |
| ---------------- | -------------------------------------- |
| `RowIndex`       | Index of the measurement               |
| `FrequencyHz`    | Measurement frequency                  |
| `R_ohm`          | Resistance                             |
| `X_ohm`          | Reactance                              |
| `V`              | Voltage                                |
| `T_degC`         | Temperature                            |
| `Range_ohm`      | Measurement range                      |
| `TimestampLocal` | Time at which the sample was processed |
| `File`           | Associated session/output path         |

---

## Sample Validation

`EisSample` performs validation when values are assigned.

### Frequency

Frequency must be greater than zero:

```text
FrequencyHz > 0
```

Otherwise a `ValidationFault` is generated.

### Resistance

Resistance must be within:

```text
0.01 Ω ≤ R_ohm ≤ 2.5 Ω
```

Invalid values generate a `ValidationFault`.

### Data Format

CSV values are parsed using floating-point conversion.

If a value cannot be parsed, a `DataFormatFault` is generated identifying the invalid field.

---

# Range Filtering

The service separates measurements based on `Range_ohm`.

The configured limits are:

```text
Minimum: 0.2
Maximum: 3.5
```

These values are defined in `Service/App.config`:

```xml
<add key="Range_min" value="0.2"/>
<add key="Range_max" value="3.5"/>
```

A sample is accepted when:

```text
0.2 < Range_ohm < 3.5
```

Accepted measurements are written to:

```text
session.csv
```

Measurements outside the configured range are written to:

```text
reject.csv
```

with the rejection reason:

```text
RangeMismatch
```

---

# Temperature Spike Detection

The service monitors temperature changes between consecutive samples.

The configured maximum temperature difference is:

```text
3 °C
```

from:

```xml
<add key="T_threshold" value="3"/>
```

The service compares the current sample against the previous sample.

If:

```text
CurrentTemperature - PreviousTemperature > 3
```

a rising temperature warning is generated.

If:

```text
CurrentTemperature - PreviousTemperature < -3
```

a falling temperature warning is generated.

The warning contains:

* Current temperature
* Temperature difference
* Frequency
* State of Charge

Example:

```text
Raising temperature.
Temperature: 30,
Difference: 4,
Frequency: 1000,
SoC: 50
```

Temperature warnings do **not** automatically reject the sample. They are reported through the event system while normal session processing continues.

---

# Event System

The project contains a small event-based notification system.

`EventGenerator` exposes four events:

```csharp
OnTransferStarted
OnSampleRecieved
OnTransferCompleted
OnTemperatureSpike
```

These events are handled by `EventListener`.

Currently, the listener writes the event message to the console.

### Event flow

```text
BatteryService
      │
      ▼
EventGenerator
      │
      ├── Transfer Started
      │
      ├── Sample Received
      │
      ├── Temperature Spike
      │
      └── Transfer Completed
              │
              ▼
        EventListener
              │
              ▼
          Console
```

This keeps event generation separate from the code responsible for displaying/logging those events.

---

# Output Files

The service creates a `Data` directory relative to its execution directory.

Each session creates two files.

## `session.csv`

Contains measurements that pass the configured range check.

The header is:

```text
RowIndex,FrequencyHz,R_ohm,X_ohm,V,T_degC,Range_ohm,TimestampLocal
```

Example:

```text
0,1000,0.15,-0.02,3.2,25,1.5,2026-10-05 19:30:00
```

---

## `reject.csv`

Contains measurements rejected by the range filter.

The header is:

```text
Timestamp,reason,RowIndex
```

Example:

```text
Timestamp,reason,RowIndex
2026-10-05 19:30:00,rejected: RangeMismatch,42
```

---

# Architecture

The project follows a simple **client-server architecture** with a shared contract/model library.

```text
┌─────────────────────────────────────────────────────┐
│                      Client                         │
│                                                     │
│  Program                                            │
│    │                                                │
│    ├── EisMeta.CreateMeta()                         │
│    │                                                │
│    ├── SampleReader                                 │
│    │                                                │
│    └── IBattery proxy                               │
└──────────────────────┬──────────────────────────────┘
                       │
                       │ WCF / Net.TCP
                       │
                       ▼
┌─────────────────────────────────────────────────────┐
│                     Service                         │
│                                                     │
│  BatteryService                                     │
│    │                                                │
│    ├── StartSession                                 │
│    ├── PushSample                                   │
│    ├── EndSession                                   │
│    │                                                │
│    ├── Temperature monitoring                       │
│    ├── Event generation                             │
│    └── SessionWriter                                │
└─────────────────────────────────────────────────────┘
                       ▲
                       │
                       │ Shared types/contracts
                       │
                ┌──────┴──────┐
                │    Common   │
                │             │
                │ IBattery    │
                │ EisMeta     │
                │ EisSample   │
                │ SampleReader│
                │ SessionWriter
                │ Events      │
                └─────────────┘
```

---

# Project Structure

```text
VP_Projekat/
│
├── Client/
│   ├── Program.cs
│   ├── App.config
│   └── Client.csproj
│
├── Service/
│   ├── BatteryService.cs
│   ├── Program.cs
│   ├── App.config
│   └── Service.csproj
│
├── Common/
│   ├── IBattery.cs
│   ├── EisMeta.cs
│   ├── EisSample.cs
│   ├── SampleReader.cs
│   ├── SessionWriter.cs
│   ├── EventGenerator.cs
│   ├── EventListener.cs
│   │
│   ├── Events/
│   │   ├── SampleRecievedEventArgs.cs
│   │   ├── SessionStartEventArgs.cs
│   │   ├── TransferCompleteEventArgs.cs
│   │   └── WarningEventArgs.cs
│   │
│   ├── DataFormatFault.cs
│   ├── ValidationFault.cs
│   └── Common.csproj
│
├── Dataset/
│   ├── B01/
│   ├── B02/
│   ├── ...
│   └── B11/
│
└── VP_Projekat.sln
```

### Client

Responsible for:

* Selecting a dataset
* Reading measurements
* Creating samples
* Communicating with the WCF service

### Service

Responsible for:

* Receiving measurements
* Processing samples
* Temperature monitoring
* Range validation
* Session output
* Event generation

### Common

Contains all types shared between the client and service.

This includes the WCF contract, DTO-like data models, file readers/writers, validation exceptions, and event infrastructure.

### Dataset

Contains the battery EIS measurement files used by the client.

---

# Communication

The application uses **Windows Communication Foundation (WCF)** with the `netTcpBinding`.

The service endpoint is:

```text
net.tcp://localhost:9000/BatteryService
```

The client connects using the endpoint configured in `Client/App.config`.

The binding uses:

```xml
<netTcpBinding>
    <binding
        name="streaming"
        transferMode="Streamed"
        maxReceivedMessageSize="67108864"
        sendTimeout="00:10:00"
        receiveTimeout="00:10:00">
        <security mode="None"/>
    </binding>
</netTcpBinding>
```

Important settings:

| Setting              | Value             |
| -------------------- | ----------------- |
| Protocol             | Net.TCP           |
| Host                 | `localhost`       |
| Port                 | `9000`            |
| Endpoint             | `/BatteryService` |
| Transfer mode        | Streamed          |
| Maximum message size | 64 MB             |
| Security             | None              |
| Send timeout         | 10 minutes        |
| Receive timeout      | 10 minutes        |

---

# Getting Started

## Requirements

The project targets:

```text
.NET Framework 4.7.2
```

You will need:

* Windows
* Visual Studio 2019 or newer
* .NET Framework 4.7.2 Developer Pack
* WCF support
* The included dataset

The project uses traditional `.csproj` files and does not require external NuGet packages.

---

## 1. Clone the repository

```bash
git clone https://github.com/nikolasovanj/VP_Projekat.git
cd VP_Projekat
```

---

## 2. Open the solution

Open:

```text
VP_Projekat.sln
```

in Visual Studio.

The solution contains:

```text
Client
Service
Common
```

---

## 3. Build the solution

Use:

```text
Build → Build Solution
```

or:

```text
Ctrl + Shift + B
```

All three projects target `.NET Framework 4.7.2`.

---

# Running the Application

The **Service must be started before the Client**.

The client expects a WCF endpoint at:

```text
net.tcp://localhost:9000/BatteryService
```

### Step 1 — Start Service

Run the `Service` project.

You should see:

```text
Service is open, press any key to close it.
```

The service is now listening on port `9000`.

### Step 2 — Start Client

Run the `Client` project separately.

The client displays:

```text
[ANY]. Send samples
2. Exit
Choose action:
```

Enter anything other than `2` to begin a transfer.

The client will automatically select a random battery dataset and begin sending its measurements.

---

# Example Run

A typical execution looks like:

```text
SERVICE

Service is open, press any key to close it.

Transfer started: /B01/Test_1/50%
Initialized transfer for sample 0
Initialized transfer for sample 1
Initialized transfer for sample 2
...
Transfer complete: /B01/Test_1/50%
```

While this is happening, the client performs:

```text
Dataset
   ↓
Select random battery/test/file
   ↓
Read CSV
   ↓
Create EisSample
   ↓
PushSample()
   ↓
Service
   ↓
Validate + process
   ↓
Write session.csv / reject.csv
```

---

# Configuration

The service configuration can be found in:

```text
Service/App.config
```

## Session messages

```xml
<add key="StartSession" value="Transfer started: "/>
<add key="SampleRecieved" value="Initialized transfer for sample "/>
<add key="EndSession" value="Transfer complete: "/>
```

These control the messages generated by the event system.

## Range validation

```xml
<add key="Range_min" value="0.2"/>
<add key="Range_max" value="3.5"/>
```

These define the accepted `Range_ohm` interval.

## Temperature threshold

```xml
<add key="T_threshold" value="3"/>
```

This controls the maximum allowed temperature difference between consecutive samples before a warning is generated.

---

# Data Flow in Detail

A complete measurement transfer looks like this:

```text
1. Client
   │
   │ Generate metadata
   ▼
2. EisMeta
   │
   │ BatteryId / TestId / SoC / FileName
   ▼
3. SampleReader
   │
   │ Read next CSV row
   ▼
4. EisSample
   │
   │ Validate / parse values
   ▼
5. WCF proxy
   │
   │ PushSample(sample)
   ▼
6. BatteryService
   │
   ├── Generate SampleReceived event
   │
   ├── Compare temperature
   │
   ├── Detect temperature spike
   │
   └── SessionWriter.Write()
   │
   ├───────────────┐
   ▼               ▼
session.csv    reject.csv
```

After the final sample:

```text
Client
  │
  ▼
EndSession()
  │
  ▼
BatteryService
  │
  ├── Close session files
  │
  └── TransferCompleted event
```

---

# Error Handling

The shared project defines two custom exceptions.

## `DataFormatFault`

Used when a CSV value cannot be parsed into the expected type.

For example:

```text
Field FrequencyHz does not have a valid format.
```

## `ValidationFault`

Used when a parsed value violates its domain constraints.

For example:

```text
Field FrequencyHz does not have a valid value.
```

These provide a distinction between:

```text
Invalid format
```

and:

```text
Valid format, invalid value
```

---

# Technologies

* **C#**
* **.NET Framework 4.7.2**
* **WCF**
* **Net.TCP**
* **Console applications**
* **DataContract / OperationContract**
* **CSV file processing**
* **Event-driven processing**
* **File streams**

No external database is required. Input and output are file-based.

---

# Design Summary

The project intentionally separates communication contracts, client functionality, and service-side processing.

```text
Common
   │
   ├── Defines contract
   ├── Defines transferred data
   ├── Reads samples
   ├── Writes sessions
   └── Defines events
       │
       ├──────────────┐
       ▼              ▼
    Client          Service
       │              │
       │              ├── Validation
       │              ├── Filtering
       │              ├── Temperature monitoring
       │              └── Output
       │
       └── Sends samples
```

This allows the client and service to share the same data contracts while keeping dataset reading and server-side processing separate.
