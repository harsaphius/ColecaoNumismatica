using System;

namespace ColecaoNumismatica
{
    public class Money
    {
        public int cod { get; set; }
        public string titulo { get; set; }
        public decimal valorCunho { get; set; }
        public decimal valorAtual { get; set; }
        public string descricao { get; set; }
        public string tipo { get; set; }
        public string estado { get; set; }
        public string imagem { get; set; }
    }

}