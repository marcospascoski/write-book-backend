using Onix.Framework.Infra.Data.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.ViewModels.Permissao
{
    public class FiltroPermissaoViewModel : Paged
    {
        public string Texto { get; set; }

        protected FiltroPermissaoViewModel() { }

        public FiltroPermissaoViewModel(
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
