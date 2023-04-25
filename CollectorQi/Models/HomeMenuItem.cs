﻿namespace CollectorQi.Models
{
    public enum MenuItemType
    {
        Cadastros,
        Recebimento,
        Estoque,
        Coletor,
        Controle,
        QualiIT
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
