using DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Business.Interfaces
{
    public interface IFornecedorRepository : IRepository<Fornecedor>
    {
        ValueTask<Fornecedor> ObterFornecedorEndereco(Guid id);
        ValueTask<Fornecedor> ObterFornecedorProdutosEndereco(Guid id);
    }
}
