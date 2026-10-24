using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1;
using WebApplication1.Models;
using Xunit;

namespace HospitalApp.Tests
{
    public class ServiceTests
    {
        private MyDbContext CreateContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new MyDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task TC01_GetAllDoctor_Should_Include_Appointments_And_Patients()
        {
            using var context = CreateContext();
            // Arrange
            var doctor = new doctor { doctorName = "Dr. Wang", special = "Cardiology" };
            context.doctor.Add(doctor);
            await context.SaveChangesAsync();

            var patient = new patient { patientName = "John Doe" };
            context.patient.Add(patient);
            await context.SaveChangesAsync();

            var appointment = new appointment
            {
                doctorID = doctor.doctorID,
                patientID = patient.patientID,
                appointmentDate = DateTime.Now,
                appointmentTime = TimeSpan.FromHours(9)
            };
            context.appointment.Add(appointment);
            await context.SaveChangesAsync();

            // Act
            var service = new Service(context);
            var result = await service.GetAllDoctor();

            // Assert
            Assert.Single(result);
            var doc = result.First();
            Assert.NotNull(doc.appointment);
            Assert.NotEmpty(doc.appointment);
            var app = doc.appointment.First();
            Assert.NotNull(app.patient);
            Assert.Equal("John Doe", app.patient.patientName);
        }

        [Fact]
        public async Task TC02_GetAppointmentswithconsoltation_Should_Only_Return_Consulted()
        {
            using var context = CreateContext();
            // Arrange
            var doc = new doctor { doctorName = "Dr. Li" };
            var pat1 = new patient { patientName = "Alice" };
            var pat2 = new patient { patientName = "Bob" };
            context.doctor.Add(doc);
            context.patient.AddRange(pat1, pat2);
            await context.SaveChangesAsync();

            var app1 = new appointment { doctorID = doc.doctorID, patientID = pat1.patientID };
            var app2 = new appointment { doctorID = doc.doctorID, patientID = pat2.patientID };
            context.appointment.AddRange(app1, app2);
            await context.SaveChangesAsync();

            var con = new consultation { appointmentID = app1.appointmentID, patientID = pat1.patientID, doctorID = doc.doctorID };
            context.consultation.Add(con);
            await context.SaveChangesAsync();

            // Act
            var service = new Service(context);
            var result = await service.GetAppointmentswithconsoltation();

            // Assert
            Assert.Single(result);
            Assert.Equal("Alice", result.First().patient.patientName);
        }

        [Fact]
        public async Task TC03_doctor_Atodoctor_Should_Create_Doctor_And_User()
        {
            using var context = CreateContext();
            var data = new JObject
            {
                ["doctorName"] = "Dr. Test",
                ["special"] = "Dermatology",
                ["username"] = "testdoc",
                ["password_hash"] = "123456",
                ["status"] = "ACTIVE"
            };

            var service = new Service(context);
            var result = await service.doctor_Atodoctor(data);

            Assert.NotNull(result);
            Assert.Equal("Dr. Test", result.doctorName);
            Assert.NotEqual(0, result.doctorID);

            var user = await context.USER_ACCOUNT.FirstOrDefaultAsync(u => u.role_id == result.doctorID);
            Assert.NotNull(user);
            Assert.Equal("DOCTOR", user.role);
            Assert.Equal("testdoc", user.username);
        }

        [Fact]
        public async Task TC04_DeleteAppointment_Should_Remove_Related_Consultations()
        {
            using var context = CreateContext();
            // Arrange
            var doc = new doctor { doctorName = "Doc" };
            var pat = new patient { patientName = "Pat" };
            context.doctor.Add(doc);
            context.patient.Add(pat);
            await context.SaveChangesAsync();

            var app = new appointment { doctorID = doc.doctorID, patientID = pat.patientID };
            context.appointment.Add(app);
            await context.SaveChangesAsync();

            var con = new consultation { appointmentID = app.appointmentID, patientID = pat.patientID, doctorID = doc.doctorID };
            context.consultation.Add(con);
            await context.SaveChangesAsync();

            var appId = app.appointmentID;
            var conId = con.consultationID;

            var service = new Service(context);
            var deleted = await service.appointment_DeleteAppointment(appId);

            Assert.True(deleted);
            Assert.Null(await context.appointment.FindAsync(appId));
            Assert.Null(await context.consultation.FindAsync(conId));
        }

        [Fact]
        public async Task TC05_GetAllDoctorbyID_Should_Return_Doctor_With_Includes()
        {
            using var context = CreateContext();
            // Arrange
            var doctor = new doctor { doctorName = "Dr. Unique" };
            context.doctor.Add(doctor);
            await context.SaveChangesAsync();

            var patient = new patient { patientName = "Jane" };
            context.patient.Add(patient);
            await context.SaveChangesAsync();

            var appointment = new appointment { doctorID = doctor.doctorID, patientID = patient.patientID };
            context.appointment.Add(appointment);
            await context.SaveChangesAsync();

            var service = new Service(context);
            var result = await service.GetAllDoctorbyID(doctor.doctorID);

            Assert.Single(result);
            Assert.Equal("Dr. Unique", result.First().doctorName);
            Assert.NotEmpty(result.First().appointment);
        }

        [Fact]
        public async Task TC06_user_UpdateUser_Should_Change_Status()
        {
            using var context = CreateContext();
            // Arrange
            var user = new USER_ACCOUNT
            {
                username = "update_test",
                password_hash = "hash",
                role = "RECEPTIONIST",
                status = "ACTIVE"
            };
            context.USER_ACCOUNT.Add(user);
            await context.SaveChangesAsync();

            var updateData = new JObject { ["status"] = "INACTIVE" };
            var service = new Service(context);
            var updated = await service.user_UpdateUser(user.user_id, updateData);

            Assert.NotNull(updated);
            Assert.Equal("INACTIVE", updated.status);
        }
    }
}