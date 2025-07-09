using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace BellosoftWebApi.Core
{
    public static class DbContextExtensions
    {
        public static void SetProperty<TEntity, TProperty>(this EntityEntry<TEntity> entity, Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value)
            where TEntity : class
        {
            PropertyEntry<TEntity, TProperty> property = entity.Property(propertyExpression);
            property.CurrentValue = value;
            property.IsModified = true;
        }
    }
}
