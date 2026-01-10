using Onix.Framework.Infra.Data.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.ViewModels.PerfilPermissao
{
    public class FiltroPerfilPermissaoViewModel : Paged
    {
        public string Texto { get; set; }

        protected FiltroPerfilPermissaoViewModel() { }

        public FiltroPerfilPermissaoViewModel(
            string propertyName,
            bool isDescending,
            int pageSize,
            int currentPage,
            string texto)
            : base(
                propertyName,
                isDescending,
                pageSize,
                currentPage)
        {
            Texto = texto;
        }
    }
}
