# Telemedicine System: Full-Stack Management & Analytics

###### A comprehensive healthcare solution featuring a C# WinForms Clinical Application for doctors, patients and a Tableau Dashboard for executive oversight. This project demonstrates a complete data lifecycle: from synthetic data generation (Python) and relational modeling (PostgreSQL) to application logic (C#/.NET) and business intelligence (Tableau).

## Project Overview
###### This project simulates a professional healthcare ecosystem. I built a C# (WinForms) Telemedicine Application as the primary data collection tool, backed by a PostgreSQL relational database. To demonstrate large-scale analysis, I engineered a Python-based data synthesis engine to generate 6,000+ realistic clinical transactions, which I then transformed into a Strategic Executive Dashboard in Tableau.

### The Problem:
###### Healthcare providers often struggle with "Data Silos"—where clinical data (vitals), operational data (appointments), and financial data (revenue) are disconnected.

### The Solution:
###### A unified pipeline that captures real-time doctor-patient interactions and converts them into actionable insights for hospital administrators.

## Part 1: C# Medical App
### Key Features:
* **Secure Doctor Login:** Personalized profile loading including specialty, experience, and hourly rates.
* **Dynamic Statistics:** Real-time calculation of total patients, work hours, and total earnings.
* **Drill-Down Data Exploration:** Contextual navigation allowing doctors to view specific diagnoses, prescriptions, or vital signs.
* **State-Aware UI:** Built a dynamic "Smart Tab" system where clinical labels only appear if specific database records (Prescriptions/Vitals) exist.

### Technical Challenges & Solutions:
#### The "Strict Type" Challenge (C# & Npgsql)
PostgreSQL's numeric type is designed for high financial precision. The Npgsql driver is "strict," requiring direct mapping to C# decimal. I implemented a robust property-based approach in the Main_Screen_Doctor form to handle currency conversion and precision without losing data during UI updates.

#### Dynamic UI State Management
* **Smart Tabs:** "Prescription" and "Vital Signs" labels act as dynamic tabs that change visibility based on the existence of database records using EXISTS subqueries.
* **Contextual Drill-Down:** Implemented event-driven logic where clicking a patient name filters the entire data context to that specific history.

### Doctor Profile
https://github.com/user-attachments/assets/33b758dd-ae48-440d-91e1-567313036fcc

### Patient Profile

https://github.com/user-attachments/assets/f706d966-b8fd-405b-b85a-7c1f6c59ec3b

## Part 2: Tableau Strategic Dashboard
### Key Business KPIs & Strategic Pillars
The system tracks three strategic areas to ensure clinic health:
1. Financial & Growth
  * **Total Revenue & ARPA:** Tracks top-line growth and the average value of each consultation.
  * **Monthly Revenue Trend & Revenue by Specialty:** Identifies seasonal growth patterns and highlights high-performing departments, with Pediatrics and Dermatology currently leading in revenue generation.
  * **Customer Acquisition Cost (CAC):** A dynamic metric correlating marketing spend with unique patient registrations.

2. Operational Efficiency
  * **Provider Utilization Rate:** Measures active consultation minutes against a 40-hour weekly capacity to identify burnout.
  * **Appointment Status Breakout:** Tracks clinic throughput by comparing Completed appointments against Scheduled, No Show and Cancelled statuses to minimize revenue leakage.
  * **Doctors Experience Years:** A histogram showing the experience distribution (Experience Years) of the medical staff.

3. Patient Care & Quality
  * **Prescribed Medicine Distribution:** Analyzes treatment trends, showing high volumes for chronic and acute care medications like Tretinoin Cream and Lisinopril.
  * **Patient Satisfaction:** Visualizes Likert Scale ratings (1-5) to identify service polarization.
  * **Referral Conversion:** Aggregates laboratory, specialist, and physiotherapy referrals by specialty—critical for identifying "Gaps in Care" and ensuring comprehensive follow-up protocols.
### Dashboard
<img width="1405" height="802" alt="Dashboard" src="https://github.com/user-attachments/assets/78f64064-62c4-4443-85a1-4ef8adb3a6cd" />
Link: https://public.tableau.com/app/profile/olha.kanikovska/viz/Telemedicinesystem/Telemedicinesystem

## Technical Architecture
* **Data Generation:** Python (Faker + Pandas) script creates a year’s worth of logical healthcare history (correlating patient age with specific diagnoses and vitals).
* **Transactional Layer:** A C# .NET application using Npgsql to manage doctor workflows, patient check-ins, and medical record updates.
* **Storage Layer:** PostgreSQL schema optimized with strict numeric typing for financial precision and EXISTS subqueries for UI performance.
* **Analytics Layer:** Tableau dashboard utilizing LOD expressions and parameter-driven "What-If" analysis.

## Tech Stack
* **Languages:** C#, SQL, Python
* **Database:** PostgreSQL 18
* **Frameworks:** .NET WinForms, ADO.NET (Npgsql)
* **Libraries:** Pandas, Faker (Data Synthesis)
* **BI Tools:** Tableau Public

## How to Run
1. Database:
   * Open your PostgreSQL terminal or pgAdmin.
   * Execute the schema definition:
```
psql -U username -d dbname -f /data/telemed_schema.sql
```

3. Synthetic Data Synthesis (Python):
   * Install dependencies:
```
pip install -r requirements.txt
```
  * Execute Data Synthesis:
    Run the script to generate high-fidelity .csv datasets for each relational entity.
```
python /data_script/generate_data.py
```

3. Bulk Data Ingestion:
Utilize the PostgreSQL COPY command for high-performance bulk loading of the generated CSV records into the database.
```
-- Repeat for each table (doctors, patients, appointments, etc.)
COPY table_name FROM 'C:\absolute\path\to\your\file.csv' DELIMITER ',' CSV HEADER;
```

5. App:
  * **Open Solution:** Launch the .sln file in Visual Studio.
  * **Connection String Management:**
    * **App.config:** Update the <connectionStrings> section with your localized PostgreSQL credentials.
    * **Controller Layer:** Navigate to the Controller folder and update the _connStr variable within the ConnectionString.cs file to match your environment.
  * Run the solution.

6. Analytics: [[Link to Tableau Public Dashboard]](https://public.tableau.com/app/profile/olha.kanikovska/viz/Telemedicinesystem/Telemedicinesystem)

Author: Olha Kanikovska

Role: Junior Data Analyst
