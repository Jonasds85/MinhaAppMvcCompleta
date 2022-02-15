﻿using AutoMapper;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.App.Controllers
{
    public class ProdutosController : BaseController
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

        public async Task<IActionResult> Index()
        {            
            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtorepository.ObterProdutosFornecedores()));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null)            
                return NotFound();

            return View(produtoViewModel);
        }

        public async Task<IActionResult> Create()
        {
            var produtoViewModel = await PopularFornecedores(new ProdutoViewModel());
            return View(produtoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid)
                return View(await PopularFornecedores(produtoViewModel));

            var produto = _mapper.Map<Produto>(produtoViewModel);
            await _produtorepository.Adicionar(produto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null)
                return NotFound();            
            
            return View(produtoViewModel);
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)            
                return NotFound();            

            if (!ModelState.IsValid)
                return View(produtoViewModel);

            var produto = _mapper.Map<Produto>(produtoViewModel);
            await _produtorepository.Atualizar(produto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null)            
                return NotFound();            

            return View(produtoViewModel);
        }

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

    }
}
