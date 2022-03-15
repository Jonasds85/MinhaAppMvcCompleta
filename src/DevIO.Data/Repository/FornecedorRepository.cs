using DevIO.Business.Interfaces;
using DevIO.Business.Interfaces.Repository;
using DevIO.Business.Models;
using DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Data.Repository
{
    public class FornecedorRepository : Repository<Fornecedor>, IFornecedorRepository
    {
        public FornecedorRepository(AppDbContext context) : base(context) { }

        public async ValueTask<Fornecedor> ObterFornecedorEndereco(Guid id)
        {
            return await Db.Fornecedores.AsNoTracking()
                                        .Include(e => e.Endereco)
                                        .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async ValueTask<Fornecedor> ObterFornecedorProdutosEndereco(Guid id)
        {
            return await Db.Fornecedores.AsNoTracking()
                                        .Include(f => f.Produtos)
                                        .Include(f => f.Endereco)
                                        .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
