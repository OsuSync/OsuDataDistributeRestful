using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful.Server
{
    class RouteTemplate
    {
        struct TemplateNode
        {
            public string Key;
            public bool IsTemplate;
        };

        private List<TemplateNode> template_breaked=new List<TemplateNode>();
        public string Template { get; set; }

        public RouteTemplate(string template)
        {
            Template = template;
            template_breaked = template.Split('/')
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n=> new TemplateNode(){
                    Key = n.Replace("{", "").Replace("}", ""),
                    IsTemplate = n[0] == '{' && n[n.Length - 1] == '}'
                }).ToList();
        }

        public bool TryMatch(string path,out ParamCollection @params)
        {
            @params = new ParamCollection();
            var breaked = path.Split('/').Where(n => !string.IsNullOrWhiteSpace(n));
            int i = 0;

            if(breaked.Count()!=template_breaked.Count)
            {
                @params = null;
                return false;
            }

            foreach(var b in breaked)
            {
                TemplateNode template = template_breaked[i];

                if (template.IsTemplate)
                {
                    string key = template.Key;
                    string value = b;
                    @params.Add(key, b);
                }
                else
                {
                    if (template.Key != b)
                    {
                        @params = null;
                        return false;
                    }
                }
                i++;
            }
            return true;
        }
    }
}
