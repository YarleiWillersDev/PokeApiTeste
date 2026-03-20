using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeApiTeste.Exceptions
{
    [Serializable]
    public class PokemonNaoEncontradoException : AppException
    {
        
        public PokemonNaoEncontradoException(string message = "Nenhum pokemon encontrado para esta cor")
            : base(404, "Entidade nao processada", message) { }

        public PokemonNaoEncontradoException(string message, Exception inner)
            : base(404, "Entidade nao processada", message, inner) { }
    }   
}