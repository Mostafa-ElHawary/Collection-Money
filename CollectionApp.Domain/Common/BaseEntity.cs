using System;

namespace CollectionApp.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

        public void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return Id != Guid.Empty && other.Id != Guid.Empty && Id == other.Id;
        }

        public static bool operator ==(BaseEntity? left, BaseEntity? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(BaseEntity? left, BaseEntity? right) => !(left == right);

        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), Id);
        }
    }
}

