using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Xunit;

namespace Domain.Tests
{
    public sealed class EventLogTests
    {
        private static readonly DateTime SampleOccurredAt = new(2026, 8, 5, 10, 30, 0);

        [Fact]
        public void Create_WithFullData_SetsAllFields()
        {
            var actorId = Guid.NewGuid();
            var entityId = Guid.NewGuid();

            var eventLog = EventLog.Create(
                EventLogType.RoleDeactivated,
                SampleOccurredAt,
                "Rol 'Ventas' desactivado; removido de 2 usuario(s)",
                userId: actorId,
                entityType: nameof(Role),
                entityId: entityId);

            Assert.Equal(EventLogType.RoleDeactivated, eventLog.Type);
            Assert.Equal(SampleOccurredAt, eventLog.OccurredAt);
            Assert.Equal(actorId, eventLog.UserId);
            Assert.Null(eventLog.AttemptedEmail);
            Assert.Equal(nameof(Role), eventLog.EntityType);
            Assert.Equal(entityId, eventLog.EntityId);
            Assert.Equal("Rol 'Ventas' desactivado; removido de 2 usuario(s)", eventLog.Description);
        }

        [Fact]
        public void Create_ForFailedLoginWithUnresolvedUser_HasNullUserIdAndEntityFields()
        {
            var eventLog = EventLog.Create(
                EventLogType.LoginFailed,
                SampleOccurredAt,
                "Intento de login fallido para 'nadie@ejemplo.com'",
                attemptedEmail: "nadie@ejemplo.com");

            Assert.Equal(EventLogType.LoginFailed, eventLog.Type);
            Assert.Null(eventLog.UserId);
            Assert.Equal("nadie@ejemplo.com", eventLog.AttemptedEmail);
            Assert.Null(eventLog.EntityType);
            Assert.Null(eventLog.EntityId);
        }

        [Fact]
        public void Create_PersistsTheOccurredAtPassedByTheCaller()
        {
            // La entidad no debe fijar su propia fecha — la fecha fiscal la decide el llamador
            // (IFiscalClock.VenezuelaNow en producción), igual que ExchangeRate.RegisterDate.
            var venezuelaNow = new DateTime(2026, 1, 1, 23, 0, 0);

            var eventLog = EventLog.Create(EventLogType.LoginSucceeded, venezuelaNow, "Login exitoso");

            Assert.Equal(venezuelaNow, eventLog.OccurredAt);
        }

        [Fact]
        public void Create_GeneratesUniqueIds()
        {
            var first = EventLog.Create(EventLogType.UserRegistered, SampleOccurredAt, "Usuario registrado");
            var second = EventLog.Create(EventLogType.UserRegistered, SampleOccurredAt, "Usuario registrado");

            Assert.NotEqual(first.Id, second.Id);
        }
    }
}
