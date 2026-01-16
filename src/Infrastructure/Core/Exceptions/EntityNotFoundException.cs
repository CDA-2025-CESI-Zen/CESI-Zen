namespace CesiZen.Infrastructure.Core.Exceptions;
public class EntityNotFoundException : Exception {
    
    public Type EntityType { get; }
    public Id?  Id         { get; }

    public EntityNotFoundException(Type type, Id id)
        : base($"Il n'y a pas d'entité de type `{type.Name}` avec l'identifiant {id} !") {
            this.EntityType = type;
            this.Id         = id;
        }

    public EntityNotFoundException(Type type)
        : base($"Il n'y a pas d'entité de type `{type.Name}` correspondante !") {
            this.EntityType = type;
        }
}