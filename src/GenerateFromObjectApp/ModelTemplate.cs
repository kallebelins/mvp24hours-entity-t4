using Mvp24Hours.Entity.Core.Settings;

namespace GenerateFromObjectApp
{
    public partial class ModelTemplate : ITextTemplate
    {
        public readonly ResultSettings Models;
        public readonly EntitySettings Entity;

        public ModelTemplate(ResultSettings models, EntitySettings entity)
            => (Models, Entity) = (models, entity);
    }
}
