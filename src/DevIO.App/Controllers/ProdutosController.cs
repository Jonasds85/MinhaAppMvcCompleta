
using AutoMapper;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Interfaces.Repository;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.App.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly IProdutoRepository _produtorepository;
        private readonly IFornecedorRepository _fornecedorpository;
        private readonly IMapper _mapper;

        public ProdutosController(
            IProdutoRepository produtorepository,
            IFornecedorRepository fornecedorpository,
            IMapper mapper)
        {
            _produtorepository = produtorepository;
            _fornecedorpository = fornecedorpository;
            _mapper = mapper;
        }

        [Route("lista-de-produtos")]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtorepository.ObterProdutosFornecedores()));           
        }

        [Route("dados-do-produtos/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null)
                return NotFound();

            return View(produtoViewModel);
        }

        [Route("novo-produto")]
        public async Task<IActionResult> Create()
        {
            var produtoViewModel = await PopularFornecedores(new ProdutoViewModel());
            return View(produtoViewModel);
        }

        [Route("novo-produto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid)
                return View(await PopularFornecedores(produtoViewModel));

            string fileName = $"{Guid.NewGuid()}_{produtoViewModel.ImagemUpload?.FileName.Replace(" ", "")}";
            if (! await UploadArquivo(produtoViewModel, fileName))            
                return View(produtoViewModel);

            produtoViewModel.Imagem = fileName;
            var produto = _mapper.Map<Produto>(produtoViewModel);            

            await _produtorepository.Adicionar(produto);
            return RedirectToAction(nameof(Index));
        }

        [Route("editar-produto/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (Guid.Empty == id)
                return RedirectToAction(nameof(Index));

            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null)
                return NotFound();

            return View(produtoViewModel);
        }

        [Route("editar-produto/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)
                return NotFound();

            var produtoAtualizacao = await ObterProduto(id);
            produtoViewModel.Fornecedor = produtoAtualizacao.Fornecedor;
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid)
                return View(produtoViewModel);

            if (produtoViewModel.ImagemUpload != null)
            {
                string fileName = $"{Guid.NewGuid()}_{produtoViewModel.ImagemUpload?.FileName.Replace(" ", "")}";
                if (!await UploadArquivo(produtoViewModel, fileName))
                    return View(produtoViewModel);

                produtoAtualizacao.Imagem = fileName;
            }

            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;
            var produto = _mapper.Map<Produto>(produtoAtualizacao);

            await _produtorepository.Atualizar(produto);
            return RedirectToAction("Index");
        }

        [Route("excluir-produto/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null)
                return NotFound();

            return View(produtoViewModel);
        }

        [Route("excluir-produto/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null)
                return NotFound();

            await _produtorepository.Remover(id);
            return RedirectToAction("Index");
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            var produto = _mapper.Map<ProdutoViewModel>(await _produtorepository.ObterProdutoFornecedor(id));
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorpository.ObterTodos());
            return produto;
        }

        private async Task<ProdutoViewModel> PopularFornecedores(ProdutoViewModel produto)
        {
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorpository.ObterTodos());
            return produto;
        }

        private async Task<bool> UploadArquivo(ProdutoViewModel produtoViewModel, string fileName)
        {
            if (produtoViewModel.ImagemUpload.Length <= 0)
                return false;
            
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", fileName);

            if (System.IO.File.Exists(path))
            {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome!");
                return false;
            }
                
            using(var stream = new FileStream(path, FileMode.Create))
            {
                await produtoViewModel.ImagemUpload.CopyToAsync(stream);
            }

            return true;
        }
    }
}
