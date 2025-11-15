---

# 📦 Multiple Barcode Printing System

**Windows Forms (.NET Framework 4.8) – MSSQL & Oracle Supported**

This project is a **Windows Forms based barcode generation and printing system**.
It supports:

* ✔ **MSSQL Database**
* ✔ **Oracle Database**
* ✔ **PDF Export (iTextSharp)**
* ✔ **Direct Print / Print Preview**
* ✔ **1.5 × 1 inch Label Size**
* ✔ **Barcode128 Standard**
* ✔ **Search by Report ID**

---

## 🚀 Features

### 🔍 1. Search Reports

Search data by `ReportId` from **MSSQL** or **Oracle** depending on the connection used in *App.config*.

### 🖨 2. Print Individual Barcode

Each row contains a **Print** button → Opens Print Preview → Can print directly.

### 🖨 3. Print All Barcodes

Prints all filtered rows together in multiple pages.

### 📄 4. Export PDF (Optional version)

There are two print versions:

| Version                           | Purpose                                              |
| --------------------------------- | ---------------------------------------------------- |
| **PDF Export Version**            | Saves barcode labels as PDF using iTextSharp         |
| **Direct Print Version (MSSQL)**  | Generates bitmap labels and prints via PrintDocument |
| **Direct Print Version (Oracle)** | Same but data source is Oracle                       |

---

## 🧩 Project Architecture

### ✔ Program.cs

* Entry point
* STAThread
* Loads Form1
* Shows startup message (“Program started!”)

### ✔ App.config

Supports both MSSQL & Oracle.

#### MSSQL Example (Commented)

```xml
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Data Source=SERVER;Initial Catalog=BarcodeDB;Integrated Security=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

#### Oracle Example (Active)

```xml
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="User Id=MultipleBarcodeDb;Password=xxxx;
       Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))
       (CONNECT_DATA=(SERVICE_NAME=ORCL)));"
       providerName="Oracle.ManagedDataAccess.Client" />
</connectionStrings>
```

---

## 🧱 Code Patterns Used

### ✔ **1. Database Access Layer**

* `SqlConnection`, `SqlCommand`, `SqlDataReader` (MSSQL)
* `OracleConnection`, `OracleCommand`, `OracleDataReader` (Oracle)
* Parameterized Queries (Safe from SQL Injection)

### ✔ **2. Barcode Generation**

Using **Barcode128 (iTextSharp)**

```csharp
Barcode128 code128 = new Barcode128 {
    Code = reportCode,
    BarHeight = 40f,
    X = 1f
};
```

### ✔ **3. PDF Creation**

Using **iTextSharp Document, PdfWriter, PdfContentByte**

### ✔ **4. Direct Print**

Using:

* `PrintDocument`
* `PrintPreviewDialog`
* Dynamic Bitmap Rendering (`Graphics`)

### ✔ **5. UI Layer**

* WinForms
* DataGridView with dynamic button column
* TextBox search
* Button to print all

---

## 🖥 User Interface Flow

1. Enter `ReportId`
2. Click **Search**
3. Data loads in grid
4. Options:

   * Print individual row
   * Print all rows
   * (Optional version) Export PDF

---

## 🗄 Database Table Structure

Your application expects:

| Column     | Example                      |
| ---------- | ---------------------------- |
| ReportId   | 001                          |
| ReportCode | A1234567890                  |
| ReportName | CBC Test                     |
| Barcode    | (Usually same as ReportCode) |

---

## 🔌 Switching Database (MSSQL ↔ Oracle)

To switch:

### ▶ MSSQL:

* Enable MSSQL `<connectionStrings>`
* Comment Oracle section
* Remove Oracle code version

### ▶ Oracle:

* Enable Oracle `<connectionStrings>`
* Use Oracle version of Form1

---

## 📦 Dependencies

| Package                      | Purpose                     |
| ---------------------------- | --------------------------- |
| **iTextSharp**               | PDF & Barcode128 generation |
| **Oracle.ManagedDataAccess** | Oracle DB driver            |
| **System.Data.SqlClient**    | MSSQL driver                |
| **System.Drawing**           | Print rendering             |

---

## 📸 Label Size

* **Outer size:** 1.5 inch × 1.0 inch
* **DPI:** 300
* **Barcode area:** 60%
* **Text area:** 40%

---

## 📂 Folder Structure (Suggested)

```
📁 MultipleBarcode
 ┣ 📁 Forms
 ┃ ┗ Form1.cs
 ┣ 📁 Services
 ┃ ┣ BarcodeGenerator.cs
 ┃ ┗ PrintService.cs
 ┣ Program.cs
 ┣ App.config
 ┗ README.md
```

---

## ▶ How to Run

1. Clone the repository
2. Restore NuGet packages
3. Update database info in **App.config**
4. Run the project in **Release mode** for clean printing
5. Connect the barcode printer
6. Print labels

---

## 📝 Notes

* Works on **Windows 10/11**
* .NET Framework **4.8** required
* Supports any **thermal barcode printer** (Zebra, Honeywell, Epson)
* Standard format: **Code128**

---

## 🧑‍💻 Author

**A N Shuvo**

---
