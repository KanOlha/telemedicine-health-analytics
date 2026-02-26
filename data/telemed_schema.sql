CREATE TABLE public.appointments (
    appointment_id integer NOT NULL,
    patient_id integer,
    doctor_id integer,
    scheduled_at timestamp with time zone NOT NULL,
    duration_minutes integer DEFAULT 30,
    status character varying(50),
    total_cost numeric(10,2),
    created_at timestamp with time zone DEFAULT now(),
    CONSTRAINT appointments_status_check CHECK (((status)::text = ANY ((ARRAY['Scheduled'::character varying, 'Completed'::character varying, 'Cancelled'::character varying, 'No-show'::character varying])::text[])))
);

CREATE TABLE public.doctors (
    doctor_id integer NOT NULL,
    full_name character varying(255) NOT NULL,
    date_of_birth date,
    specialty character varying(100),
    experience_years integer,
    workplace character varying(255),
    email character varying(255),
    password_hash text,
    hourly_rate numeric(10,2) DEFAULT 50.00,
    created_at timestamp with time zone DEFAULT now()
);

CREATE TABLE public.feedback (
    feedback_id integer NOT NULL,
    appointment_id integer,
    rating integer,
    comments text,
    created_at timestamp with time zone DEFAULT now(),
    CONSTRAINT feedback_rating_check CHECK (((rating >= 1) AND (rating <= 5)))
);

CREATE TABLE public.patients (
    patient_id integer NOT NULL,
    full_name character varying(255) NOT NULL,
    date_of_birth date,
    gender character varying(20),
    address text,
    email character varying(255),
    password_hash text,
    created_at timestamp with time zone DEFAULT now()
);

CREATE TABLE public.prescriptions (
    prescription_id integer NOT NULL,
    record_id integer,
    medicine_name character varying(255),
    dosage character varying(100),
    frequency character varying(100),
    duration_days integer,
    created_at timestamp with time zone DEFAULT now()
);

CREATE TABLE public.visit_records (
    record_id integer NOT NULL,
    appointment_id integer,
    preliminary_diagnosis text,
    final_diagnosis_code character varying(10),
    notes text,
    referral_labs boolean DEFAULT false,
    referral_specialist boolean DEFAULT false,
    referral_physio boolean DEFAULT false
);

CREATE TABLE public.vital_signs (
    vital_id integer NOT NULL,
    appointment_id integer,
    heart_rate integer,
    blood_pressure_sys integer,
    blood_pressure_dia integer,
    temperature numeric(4,2),
    oxygen_saturation integer,
    recorded_at timestamp with time zone DEFAULT now()
);

ALTER TABLE ONLY public.appointments
    ADD CONSTRAINT appointments_pkey PRIMARY KEY (appointment_id);


ALTER TABLE ONLY public.doctors
    ADD CONSTRAINT doctors_email_key UNIQUE (email);

ALTER TABLE ONLY public.doctors
    ADD CONSTRAINT doctors_pkey PRIMARY KEY (doctor_id);


ALTER TABLE ONLY public.feedback
    ADD CONSTRAINT feedback_pkey PRIMARY KEY (feedback_id);


ALTER TABLE ONLY public.patients
    ADD CONSTRAINT patients_email_key UNIQUE (email);

ALTER TABLE ONLY public.patients
    ADD CONSTRAINT patients_pkey PRIMARY KEY (patient_id);


ALTER TABLE ONLY public.prescriptions
    ADD CONSTRAINT prescriptions_pkey PRIMARY KEY (prescription_id);


ALTER TABLE ONLY public.visit_records
    ADD CONSTRAINT visit_records_appointment_id_key UNIQUE (appointment_id);

ALTER TABLE ONLY public.visit_records
    ADD CONSTRAINT visit_records_pkey PRIMARY KEY (record_id);


ALTER TABLE ONLY public.vital_signs
    ADD CONSTRAINT vital_signs_pkey PRIMARY KEY (vital_id);


ALTER TABLE ONLY public.appointments
    ADD CONSTRAINT appointments_doctor_id_fkey FOREIGN KEY (doctor_id) REFERENCES public.doctors(doctor_id);

ALTER TABLE ONLY public.appointments
    ADD CONSTRAINT appointments_patient_id_fkey FOREIGN KEY (patient_id) REFERENCES public.patients(patient_id);


ALTER TABLE ONLY public.feedback
    ADD CONSTRAINT feedback_appointment_id_fkey FOREIGN KEY (appointment_id) REFERENCES public.appointments(appointment_id);


ALTER TABLE ONLY public.prescriptions
    ADD CONSTRAINT prescriptions_record_id_fkey FOREIGN KEY (record_id) REFERENCES public.visit_records(record_id);


ALTER TABLE ONLY public.visit_records
    ADD CONSTRAINT visit_records_appointment_id_fkey FOREIGN KEY (appointment_id) REFERENCES public.appointments(appointment_id);


ALTER TABLE ONLY public.vital_signs
    ADD CONSTRAINT vital_signs_appointment_id_fkey FOREIGN KEY (appointment_id) REFERENCES public.appointments(appointment_id);

