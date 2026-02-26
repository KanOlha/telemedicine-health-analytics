import pandas as pd
import numpy as np
from faker import Faker
from datetime import datetime, timedelta
import random
import string

#------Helper Function------
def generate_custom_id(prefix, row_index):
    return int(f"{prefix}{row_index + 1}")

def generate_email(full_name):
    clean_name = full_name.lower().replace(" ", ".")
    clean_name = clean_name.replace("dr.", "").replace("mr.", "").replace("ms.", "")
    return f"{clean_name.strip('.')}@example.com"

def generate_password(length = 8):
    characters = string.ascii_letters + string.digits + "*%$@!&"
    return ''.join(random.choice(characters) for _ in range(length))

#--------Main Functions-----------
def generate_parents(num_patients, num_doctors):
    # --- 1. Generate Doctors table (Prefix 579) ---
    doctors = []
    specialties = [
        ('Cardiology', 80, 150), 
        ('Neurology', 90, 160), 
        ('Pediatrics', 40, 70), 
        ('General Practice', 30, 60), 
        ('Dermatology', 50, 100)
    ]

    for i in range(num_doctors):
        spec_info = random.choice(specialties)
        
        # Logic: Experience correlated with age (roughly)
        age = random.randint(30, 65)
        experience = max(1, age - 25 - random.randint(0, 5))
        name = fake.name()

        doctors.append({
            'doctor_id': generate_custom_id(579, i),
            'full_name': name,
            'date_of_birth': fake.date_of_birth(minimum_age=30, maximum_age=65),
            'specialty': spec_info[0],
            'experience_years': experience,
            'workplace': f"{fake.city()} City Hospital",
            'email': generate_email(name),
            'password_hash': generate_password(),
            'hourly_rate': random.randint(spec_info[1], spec_info[2]) + (experience * 2),
            'created_at': datetime.now() - timedelta(days=random.randint(365, 730))
        })
    
    df_doctors = pd.DataFrame(doctors)

    # --- 2. Generate Patients table (Prefix 681)---
    patients = []
    for i in range(num_patients):
        name = fake.name()
        patients.append({
            'patient_id': generate_custom_id(681, i),
            'full_name': name,
            'date_of_birth': fake.date_of_birth(minimum_age=1, maximum_age=90),
            'gender': random.choice(['Male', 'Female']),
            'address': fake.address().replace('\n', ', '),
            'email': generate_email(name),
            'password_hash': generate_password(),
            'created_at': datetime.now() - timedelta(days=random.randint(1, 365))
        })
    df_patients = pd.DataFrame(patients)

    # Returns df_doctors, df_patients
    return df_doctors, df_patients
    pass


def generate_appointments(df_doctors, df_patients, num_appointments):
    # --- 3. Generate Appointments table (Prefix 753) ---
    appointments = []
    start_date = datetime(2025, 1, 1)

    # Track the "Late Doctor" anomaly
    unreliable_doctor_id = df_docs.iloc[0]['doctor_id']

    for i in range(num_appointments):
        doc = df_docs.sample(1).iloc[0]
        pat = df_pats.sample(1).iloc[0]
        
        # Generate a random time in the year 2025
        appt_time = start_date + timedelta(days=random.randint(0, 364), hours=random.randint(8, 18))
        
        # SEASONAL LOGIC: Increase Pediatrics/GP visits in Winter (Jan, Feb, Dec)
        is_winter = appt_time.month in [1, 2, 12]
        if is_winter and doc['specialty'] in ['Pediatrics', 'General Practice']:
            pass

        # STATUS LOGIC & ANOMALY
        if doc['doctor_id'] == unreliable_doctor_id:
            status = random.choices(['Completed', 'No-show', 'Cancelled'], weights=[40, 30, 30])[0]
        else:
            status = random.choices(['Completed', 'No-show', 'Scheduled', 'Cancelled'], weights=[80, 5, 10, 5])[0]

        # DURATION & COST LOGIC
        duration = random.choice([15, 30, 45, 60])
        total_cost = round((float(doc['hourly_rate']) / 60) * duration, 2)

        appointments.append({
            'appointment_id': generate_custom_id(753, i),
            'patient_id': pat['patient_id'],
            'doctor_id': doc['doctor_id'],
            'scheduled_at': appt_time,
            'duration_minutes': duration,
            'status': status,
            'total_cost': total_cost if status == 'Completed' else 0.00,
            'created_at': appt_time - timedelta(days=random.randint(1, 14))
        })

    df_appointments = pd.DataFrame(appointments)

    # Returns df_appointments
    return df_appointments
    pass

def generate_outcomes(df_doctors, df_appointments):
    # Code to generate Records, Vitals, Prescriptions, Feedback tables
    # Filter for 'Completed' appointments only - others shouldn't have clinical data
    completed_appts = df_appointments[df_appointments['status'] == 'Completed'].copy()
    
    visit_records = []
    vital_signs = []
    prescriptions = []
    feedback = []

    # Dictionary for Clinical Logic (ICD-10 Codes)
    clinical_map = {
        'Cardiology': [('I10', 'Essential Hypertension'), ('I20', 'Angina Pectoris'), ('I48', 'Atrial Fibrillation')],
        'Neurology': [('G43', 'Migraine'), ('G47', 'Insomnia'), ('R51', 'Headache')],
        'Pediatrics': [('J01', 'Acute Sinusitis'), ('J02', 'Acute Pharyngitis'), ('Z00.1', 'Routine Child Exam')],
        'General Practice': [('J11', 'Influenza'), ('E11', 'Type 2 Diabetes'), ('K21', 'GERD')],
        'Dermatology': [('L20', 'Atopic Dermatitis'), ('L70', 'Acne'), ('B35', 'Dermatophytosis')]
    }

    med_map = {
        'I10': ('Lisinopril', '10mg', 'Once daily'),
        'J11': ('Oseltamivir', '75mg', 'Twice daily'),
        'G43': ('Sumatriptan', '50mg', 'As needed'),
        'E11': ('Metformin', '500mg', 'Twice daily with meals'),
        'L70': ('Tretinoin Cream', '0.025%', 'Apply at night')
    }

    #Table 4: Visit_records (Prefix 823)
    #Table 5: Vital Signs (Prefix 892)
    #Table 6: Prescription (Prefix 945)
    #Table 7: Feedback (Prefix 909)

    for i, (_, appt) in enumerate(completed_appts.iterrows()):
        # Get doctor's specialty to determine diagnosis
        doc_spec = df_doctors[df_doctors['doctor_id'] == appt['doctor_id']]['specialty'].values[0]
        diag_choice = random.choice(clinical_map.get(doc_spec, [('R69', 'Unknown')]))
        
        rec_id = generate_custom_id(823, i)

        # 1. GENERATE VISIT RECORDS
        visit_records.append({
            'record_id': rec_id,
            'appointment_id': appt['appointment_id'],
            'preliminary_diagnosis': f"Patient reports symptoms matching {diag_choice[1].lower()}.",
            'final_diagnosis_code': diag_choice[0],
            'notes': "Standard follow-up recommended in 2 weeks.",
            'referral_labs': random.choices([True, False], weights=[30, 70])[0],
            'referral_specialist': random.choices([True, False], weights=[10, 90])[0],
            'referral_physio': random.choices([True, False], weights=[5, 95])[0]
        })


        # 2. GENERATE VITAL SIGNS
        # LOGIC: If diagnosis is Hypertension (I10), let's spike the Blood Pressure
        is_hypertensive = (diag_choice[0] == 'I10')
        vital_signs.append({
            'vital_id' : generate_custom_id(892, i),
            'appointment_id': appt['appointment_id'],
            'heart_rate': random.randint(60, 100),
            'blood_pressure_sys': random.randint(145, 180) if is_hypertensive else random.randint(110, 130),
            'blood_pressure_dia': random.randint(90, 110) if is_hypertensive else random.randint(70, 85),
            'temperature': round(random.uniform(36.1, 37.8), 1),
            'oxygen_saturation': random.randint(95, 100),
            'recorded_at': appt['scheduled_at'] + timedelta(minutes=5)
        })

        # 3. GENERATE PRESCRIPTIONS (If med exists for diagnosis)
        if diag_choice[0] in med_map:
            med = med_map[diag_choice[0]]
            prescriptions.append({
                'prescription_id' : generate_custom_id(945, i),
                'record_id': rec_id,
                'medicine_name': med[0],
                'dosage': med[1],
                'frequency': med[2],
                'duration_days': random.choice([7, 14, 30, 90]),
                'created_at': appt['scheduled_at']
            })

    # 4. GENERATE FEEDBACK (For all status types)
    for i, (_, appt )in enumerate(df_appointments.iterrows()):
        # LOGIC: High ratings for 'Completed', low for 'Cancelled'/'No-show'
        if appt['status'] == 'Completed':
            rating = random.choices([5, 4, 3], weights=[70, 20, 10])[0]
            comment = random.choice(["Great experience", "Very helpful doctor", "Seamless call", "Clear advice"])
        else:
            rating = random.choices([1, 2, 3], weights=[40, 40, 20])[0]
            comment = random.choice(["Appointment was cancelled last minute", "Waited too long", "Technical issues"])

        if random.random() > 0.5: # 50% participation rate
            feedback.append({
                'feedback_id' : generate_custom_id(909, i),
                'appointment_id': appt['appointment_id'],
                'rating': rating,
                'comments': comment,
                'created_at': appt['scheduled_at'] + timedelta(hours=random.randint(1, 24))
            })

    df_records = pd.DataFrame(visit_records)
    df_vitals = pd.DataFrame(vital_signs)
    df_prescriptions = pd.DataFrame(prescriptions)
    df_feedback = pd.DataFrame(feedback)

    # Returns df_records, df_vitals, etc.
    return df_records, df_vitals, df_prescriptions, df_feedback
    pass

fake = Faker()
num_patients = 1000
num_doctors = 50
num_appointments = 6000

# Step 1: Parents
df_docs, df_pats = generate_parents(num_patients, num_doctors)

# Step 2: Children
df_appts = generate_appointments(df_docs, df_pats, num_appointments)

# Step 3: Grandchildren
df_recs, df_vits, df_pres, df_feed = generate_outcomes(df_docs, df_appts)

# Save to CSV for easy PostgreSQL Import
df_docs.to_csv('doctors.csv', index=False)
df_pats.to_csv('patients.csv', index=False)
df_appts.to_csv('appointments.csv', index=False)
df_recs.to_csv('visit_records.csv', index=False)
df_vits.to_csv('vital_signs.csv', index=False)
df_pres.to_csv('prescriptions.csv', index=False)
df_feed.to_csv('feedback.csv', index=False)

print(f"Tables successfully generated.")