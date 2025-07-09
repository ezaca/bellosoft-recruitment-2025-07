namespace BellosoftWebApi.Services.AuthenticatedUser
{
    public class AuthUserData
    {
        public int Id { get; set; }
        public int? SelectedDeckId { get; set; }
        public DateTime? DeletedAt { get; set; }

        public AuthUserData(int id, int? selectedDeckId, DateTime updatedAt, DateTime? deletedAt)
        {
            Id = id;
            SelectedDeckId = selectedDeckId;
            DeletedAt = deletedAt;
        }

        public bool IsSoftDeleted => DeletedAt is not null && DeletedAt < DateTime.UtcNow;
    }
}
