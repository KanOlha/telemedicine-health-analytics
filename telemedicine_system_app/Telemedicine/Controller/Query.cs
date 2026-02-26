using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace Telemedicine.Controller
{
    internal class Query
    {
        private readonly string _connStr;

        public Query(string connStr)
        {
            _connStr = connStr;
        }

        //------------------------------------------------------PROFILE DOCTOR----------------------------------------------------------------

        public void SignIN_Doctors_Account(string email, string password)
        {
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT doctor_id, full_name, date_of_birth, specialty, 
                           experience_years, workplace, hourly_rate, email 
                    FROM doctors 
                    WHERE email = @email AND password_hash = @password";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@password", password);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string fullName = reader.GetString(1);
                                DateTime dob = reader.GetDateTime(2);
                                string specialty = reader.GetString(3);
                                int experience = reader.GetInt32(4);
                                string workplace = reader.GetString(5);
                                decimal hourlyRate = reader.GetDecimal(6);
                                string emailAddress = reader.GetString(7);

                                Main_Screen_Doctor newForm = new Main_Screen_Doctor(userId);
                                newForm.FullName = fullName;
                                newForm.DateOfBirth = dob.ToShortDateString();
                                newForm.Specialty = specialty;
                                newForm.Experience = experience;
                                newForm.WorkPlace = workplace;
                                newForm.HourlyRate = hourlyRate;
                                newForm.Email = emailAddress; 
                                newForm.Show();
                            }
                            else
                            {
                                MessageBox.Show("Користувача не знайдено.", "Помилка входу", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}");
                }
            }
        }

        public void SignUP_Doctor_Account(string name, string birthday, string specialty, int experience, string workPlace, string email, string password)
        {
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                    INSERT INTO doctors (full_name, date_of_birth, specialty, experience_years, workplace, email, password_hash) 
                    VALUES ((SELECT COALESCE(MAX(doctor_id), 0) + 1 FROM doctors),
                    @name, @dob, @specialty, @exp, @work, @email, @pass)";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@dob", DateTime.Parse(birthday));
                        command.Parameters.AddWithValue("@specialty", specialty);
                        command.Parameters.AddWithValue("@exp", experience);
                        command.Parameters.AddWithValue("@work", workPlace);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@pass", password);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Акаунт успішно створено!", "Реєстрація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Registration Error: {ex.Message}");
                }
            }
        }

        public DataTable Show_Patients_List(int userId)
        {
            DataTable bufferTable = new DataTable();
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT patients.full_name, patients.date_of_birth, patients.gender, patients.address
                    FROM patients
                    INNER JOIN appointments ON appointments.patient_id = patients.patient_id
                    INNER JOIN visit_records ON visit_records.appointment_id = appointments.appointment_id
                    WHERE appointments.doctor_id = @userId"; ;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(bufferTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public DataTable Show_Appointments_list_for_Doctor(int userId)
        {
            DataTable bufferTable = new DataTable();

            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT scheduled_at, patients.full_name, status
                    FROM patients
                    INNER JOIN appointments ON appointments.patient_id = patients.patient_id
                    WHERE appointments.doctor_id = @docId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@docId", userId);

                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                        dataAdapter.Fill(bufferTable);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public DataTable Show_Patient_Diagnosis(int userID, string patientName)
        {
            DataTable bufferTable = new DataTable();
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                    SELECT patients.full_name,
                    appointments.appointment_id, appointments.scheduled_at, 
                    visit_records.preliminary_diagnosis,
                    visit_records.final_diagnosis_code, visit_records.referral_labs,
                    visit_records.referral_specialist, visit_records.referral_physio
                    FROM patients
                    INNER JOIN appointments ON appointments.patient_id = patients.patient_id
                    INNER JOIN visit_records ON appointments.appointment_id = visit_records.appointment_id
                    WHERE appointments.doctor_id = @userID AND patients.full_name = @Name";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userID", userID);
                        command.Parameters.AddWithValue("@Name", patientName);
                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                        dataAdapter.Fill(bufferTable);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public DataTable Show_Patient_Prescription(int userID, int appointmentId)
        {
            DataTable bufferTable = new DataTable();
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                    SELECT patients.full_name,
                    appointments.scheduled_at, 
                    prescriptions.medicine_name, prescriptions.dosage, prescriptions.frequency, prescriptions.duration_days
                    FROM patients
                    INNER JOIN appointments ON appointments.patient_id = patients.patient_id
                    INNER JOIN visit_records ON appointments.appointment_id = visit_records.appointment_id
                    INNER JOIN prescriptions ON prescriptions.record_id = visit_records.record_id
                    WHERE appointments.doctor_id = @userID AND appointments.appointment_id = @appointId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userID", userID);
                        command.Parameters.AddWithValue("@appointId", appointmentId);
                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                        dataAdapter.Fill(bufferTable);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public DataTable Show_Patient_Vital_signs(int userID, int appointmentId)
        {
            DataTable bufferTable = new DataTable();
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                    SELECT patients.full_name,
                    appointments.scheduled_at, 
                    vital_signs.heart_rate, vital_signs.blood_pressure_sys, 
                    vital_signs.blood_pressure_dia, vital_signs.temperature, vital_signs.oxygen_saturation
                    FROM patients
                    INNER JOIN appointments ON appointments.patient_id = patients.patient_id
                    INNER JOIN vital_signs ON appointments.appointment_id = vital_signs.appointment_id
                    WHERE appointments.doctor_id = @userID AND appointments.appointment_id = @appointId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userID", userID);
                        command.Parameters.AddWithValue("@appointId", appointmentId);
                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                        dataAdapter.Fill(bufferTable);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public int Total_patient(int userId)
        {
            int count = 0;

            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT COUNT (DISTINCT patient_id)
                    FROM appointments
                    WHERE doctor_id = @docId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@docId", userId);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            count = Convert.ToInt32(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return count; 
        }

        public int Rating(int userId)
        {
            int count = 0;

            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT AVG (rating)
                    FROM feedback
                    INNER JOIN appointments ON appointments.appointment_id = feedback.appointment_id
                    WHERE doctor_id = @docId AND status = 'Completed'";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@docId", userId);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            count = Convert.ToInt32(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return count;
        }

        //---------------------------------PATIENT PROFILE----------------------------------------------------

        public void SignIN_Patients_Account(string email, string password)
        {
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                    SELECT patient_id, full_name, date_of_birth, gender, address, email 
                    FROM patients 
                    WHERE email = @email AND password_hash = @password";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@password", password); 

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string fullName = reader.GetString(1);

                                DateTime dob = reader.GetDateTime(2);
                                string gender = reader.GetString(3);
                                string address = reader.GetString(4);
                                string emailAddress = reader.GetString(5);

                                Main_Screen_Patient newForm = new Main_Screen_Patient(userId);
                                newForm.FullName = fullName;
                                newForm.DateOfBirth = dob.ToShortDateString();
                                newForm.Gender = gender;
                                newForm.Address = address;
                                newForm.Email = emailAddress;
                                newForm.Show();
                            }
                            else
                            {
                                MessageBox.Show("User doesn't exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}");
                }
            }
        }

        public void SignUP_Patient_Account(string name, string birthday, string gender, string address, string email, string password)
        {
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                    INSERT INTO patients (full_name, date_of_birth, gender, address, email, password_hash) 
                    VALUES ((SELECT COALESCE(MAX(patient_id), 0) + 1 FROM patients),
                    @name, @dob, @gender, @address, @email, @pass)";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@dob", DateTime.Parse(birthday));
                        command.Parameters.AddWithValue("@gender", gender);
                        command.Parameters.AddWithValue("@address", address);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@pass", password);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Акаунт успішно створено!", "Реєстрація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Registration Error: {ex.Message}");
                }
            }
        }

        public DataTable Show_Appointments_list_for_Patient(int userId)
        {
            DataTable bufferTable = new DataTable();

            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT scheduled_at, doctors.full_name, status
                    FROM doctors
                    INNER JOIN appointments ON appointments.doctor_id = doctors.doctor_id
                    WHERE appointments.patient_id = @patId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@patId", userId);

                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                        dataAdapter.Fill(bufferTable);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public DataTable Show_Doctors_list(int userId)
        {
            DataTable bufferTable = new DataTable();
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT doctors.full_name, doctors.date_of_birth, doctors.specialty, doctors.experience_years,
                    doctors.workplace, doctors.email
                    FROM doctors
                    INNER JOIN appointments ON appointments.doctor_id = doctors.doctor_id
                    INNER JOIN visit_records ON visit_records.appointment_id = appointments.appointment_id
                    WHERE appointments.patient_id = @userId"; ;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(bufferTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public DataTable Show_Diagnosis(int userID, string doctorName)
        {
            DataTable bufferTable = new DataTable();
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                    SELECT doctors.full_name,
                    appointments.appointment_id, appointments.scheduled_at, 
                    visit_records.preliminary_diagnosis,
                    visit_records.final_diagnosis_code, visit_records.referral_labs,
                    visit_records.referral_specialist, visit_records.referral_physio
                    FROM doctors
                    INNER JOIN appointments ON appointments.doctor_id = doctors.doctor_id
                    INNER JOIN visit_records ON appointments.appointment_id = visit_records.appointment_id
                    WHERE appointments.patient_id = @userID AND doctors.full_name = @Name";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userID", userID);
                        command.Parameters.AddWithValue("@Name", doctorName);
                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                        dataAdapter.Fill(bufferTable);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public DataTable Show_Prescription(int userID, int appointmentId)
        {
            DataTable bufferTable = new DataTable();
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                    SELECT doctors.full_name,
                    appointments.scheduled_at, 
                    prescriptions.medicine_name, prescriptions.dosage, prescriptions.frequency, prescriptions.duration_days
                    FROM doctors
                    INNER JOIN appointments ON appointments.doctor_id = doctors.doctor_id
                    INNER JOIN visit_records ON appointments.appointment_id = visit_records.appointment_id
                    INNER JOIN prescriptions ON prescriptions.record_id = visit_records.record_id
                    WHERE appointments.patient_id = @userID AND appointments.appointment_id = @appointId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userID", userID);
                        command.Parameters.AddWithValue("@appointId", appointmentId);
                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                        dataAdapter.Fill(bufferTable);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public DataTable Show_Vital_signs(int userID, int appointmentId)
        {
            DataTable bufferTable = new DataTable();
            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                    SELECT doctors.full_name,
                    appointments.scheduled_at, 
                    vital_signs.heart_rate, vital_signs.blood_pressure_sys, 
                    vital_signs.blood_pressure_dia, vital_signs.temperature, vital_signs.oxygen_saturation
                    FROM doctors
                    INNER JOIN appointments ON appointments.doctor_id = doctors.doctor_id
                    INNER JOIN vital_signs ON appointments.appointment_id = vital_signs.appointment_id
                    WHERE appointments.patient_id = @userID AND appointments.appointment_id = @appointId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userID", userID);
                        command.Parameters.AddWithValue("@appointId", appointmentId);
                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                        dataAdapter.Fill(bufferTable);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return bufferTable;
        }

        public int Total_doctors(int userId)
        {
            int count = 0;

            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT COUNT (DISTINCT doctor_id)
                    FROM appointments
                    WHERE patient_id = @patId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@patId", userId);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            count = Convert.ToInt32(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return count;
        }

        //---------------------------------GENERAL----------------------------------------------------
        public int Total_appointments(int userId)
        {
            int count = 0;

            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT COUNT (DISTINCT appointment_id)
                    FROM appointments
                    WHERE doctor_id = @userId OR patient_id = @userId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            count = Convert.ToInt32(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return count;
        }

        public int Total_minutes(int userId)
        {
            int count = 0;

            using (var connection = new NpgsqlConnection(_connStr))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                    SELECT SUM (duration_minutes)
                    FROM appointments
                    WHERE doctor_id = @userId OR patient_id = @userId AND status = 'Completed'";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            count = Convert.ToInt32(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading patients: {ex.Message}");
                }
            }
            return count;
        }

        //Addition function for checking the validity of an appointment_id exist in the tables Prescription and Vital_signs
        public bool HasPrescription(int appointmentId)
        {
            using (var conn = new NpgsqlConnection(_connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"
                    SELECT EXISTS(SELECT 1 
                    FROM prescriptions 
                    JOIN visit_records ON prescriptions.record_id = visit_records.record_id 
                    WHERE visit_records.appointment_id = @id)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", appointmentId);
                        return (bool)cmd.ExecuteScalar();
                    }
                }
                catch { return false; }
            }
        }

        public bool HasVitals(int appointmentId)
        {
            using (var conn = new NpgsqlConnection(_connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"
                    SELECT EXISTS(SELECT 1 
                    FROM vital_signs  
                    WHERE appointment_id = @id)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", appointmentId);
                        return (bool)cmd.ExecuteScalar();
                    }
                }
                catch { return false; }
            }
        }
    }
}
