using DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Business.Interfaces.Repository
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        ValueTask<IEnumerable<Produto>> ObterProdutosPorFornecedor(Guid fornecedorId);
        Task<IEnumerable<Produto>> ObterProdutosFornecedores();
        ValueTask<Produto> ObterProdutoFornecedor(Guid id);
    }
}
