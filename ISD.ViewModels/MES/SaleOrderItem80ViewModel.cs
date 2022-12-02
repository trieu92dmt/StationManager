using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class SaleOrderItem80ViewModel: SaleOrderItem80Model
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ItemDescription")]
        public string ItemDescription { get; set; }

        public List<BomDetailViewModel> BomDetail { get; set; }

        public string POSNR_DISPLAY
        {
            get
            {
                if (!string.IsNullOrEmpty(POSNR))
                {
                    return POSNR.TrimStart(new Char[] { '0' });
                }
                return string.Empty;
            }
        }

    }
}
