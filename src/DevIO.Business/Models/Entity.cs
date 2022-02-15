using System.ComponentModel;

namespace DevIO.Business.Models
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public bool Ativo { get; set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }
    }

}
