
namespace DevIO.Business.Models
{
    public class Fornecedor : Entity
    {
        public string Nome { get; set; }
        public string Documento { get; set; }
        public TipoFornecedor TipoFornecedor { get; set; }
        public Endereco? Endereco { get; set; }

        //EF relations
        public IEnumerable<Produto>? Produtos { get; set; } 

    }
}
