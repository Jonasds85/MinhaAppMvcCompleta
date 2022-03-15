using AutoMapper;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Interfaces.Repository;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.App.Controllers
{
    public class FornecedoresController : BaseController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;

        public FornecedoresController(
            IFornecedorRepository fornecedorRepository,
            IProdutoRepository produtoRepository,
            IEnderecoRepository enderecoRepository,
            IMapper mapper)
        {
            _fornecedorRepository = fornecedorRepository;
            _produtoRepository = produtoRepository;
            _enderecoRepository = enderecoRepository;
            _mapper = mapper;
        }
                
        [Route("lista-de-fornecedores")]
        public async Task<IActionResult> Index()
        {
            var fornecedoresViewModel = 
                _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return View(fornecedoresViewModel);
        }

        [Route("dados-do-fornecedor/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorEndereco(id);               
            if (fornecedorViewModel == null)            
                return NotFound();            

            return View(fornecedorViewModel);
        }

        [Route("novo-fornecedor")]
        public IActionResult Create() { return View(); }

        [Route("novo-fornecedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid)
                return View(fornecedorViewModel);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
            await _fornecedorRepository.Adicionar(fornecedor);
            return RedirectToAction("Index");
        }

        [Route("editar-fornecedor/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorProdutosEndereco(id);
            if (fornecedorViewModel == null)            
                return NotFound();
            
            return View(fornecedorViewModel);
        }

        [Route("editar-fornecedor/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id)            
                return NotFound();            

            if (!ModelState.IsValid)
                return View(fornecedorViewModel);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
            await _fornecedorRepository.Atualizar(fornecedor);            
            return RedirectToAction("Index");
        }

        [Route("excluir-fornecedor/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorProdutosEndereco(id);                
            if (fornecedorViewModel == null)            
                return NotFound();

            return View(fornecedorViewModel);
        }

        [Route("excluir-fornecedor/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorProdutosEndereco(id);
            if (fornecedorViewModel == null)
                return NotFound();

            await _fornecedorRepository.Remover(id);
            return RedirectToAction("Index");
        }

        //NÃO ESTOU USANDO ESSE GET, POIS JÁ TENHOO ENDERECO NA VIEW DE CADASTRO DE FORNECEDORES
        public async Task<IActionResult> Atualizarendereco(Guid id)
        {
            var fornecedor =  await ObterFornecedorEndereco(id);
            if (fornecedor == null)
                return NotFound();

            return PartialView("_AtualizarEndereco", new FornecedorViewModel { Endereco = fornecedor.Endereco });
        }

        [Route("atualizar-endereco-fornecedor/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Atualizarendereco(FornecedorViewModel fornecedorViewModel)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Documento");
            if (!ModelState.IsValid)
                return PartialView("_AtualizarEndereco", fornecedorViewModel);

            Endereco endereco = _mapper.Map<Endereco>(fornecedorViewModel.Endereco);
            await _enderecoRepository.Atualizar(endereco);

            var url = Url.Action("ObterEndereco", "Fornecedores", new { id = fornecedorViewModel.Endereco.FornecedorId });
            return Json(new { success = true, url = url });
        }

        [Route("obter-endereco-fornecedor/{id:guid}")]
        public async Task<IActionResult> ObterEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);
            if (fornecedor == null)
                return NotFound();

            return PartialView("_DetalhesEndereco", fornecedor);
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            var fornecedorViewModel = _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
            return fornecedorViewModel;
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }
    }
}
