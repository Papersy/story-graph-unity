using System.Text.RegularExpressions;
using ActionButtons;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UI
{
    public class AllActions : BaseWindow
    {
        [SerializeField] private ButtonAction _btnPrefab;
        [SerializeField] private Transform _contentParent;
        
        private JToken _currentProductions;
        private JToken _currentProduction;

        public override void OnShow()
        {
            _currentProductions = GameService.GetGameController().GetCurrentProductions();
            InitProductions();
            
            base.OnShow();
        }

        public void Init()
        {
            InitProductions();
        }
        

        private void InitProductions()
        {
            ClearContentView(_contentParent);
            
            foreach (var production in _currentProductions)
            {
                var btn = Instantiate(_btnPrefab, _contentParent);
                var prod = production["prod"];
                var title = prod["Title"].ToString();
                
                btn.SetText(GetPolishPart(title));
                btn.Button.onClick.AddListener(() => OnProductionClick(production));
            }
        }

        private void InitVariants()
        {
            var prod = _currentProduction["prod"];
            var variants = _currentProduction["variants"];
            var description = prod["Description"].ToString();

            foreach (var variant in variants)
            {
                var btn = Instantiate(_btnPrefab, _contentParent);
                string newDescription = description;
                
                foreach (var podVariant in variant)
                {
                    var searchWord = podVariant["LSNodeRef"].ToString();
                    var changeTo = podVariant["WorldNodeName"].ToString();
                    
                    newDescription = RenameDescription(newDescription, searchWord, changeTo);
                }
                
                btn.SetText(newDescription);
                btn.Button.onClick.AddListener(() => OnVariantClick(prod, variant));
            }
        }
        
        private void OnProductionClick(JToken production)
        {
            _currentProduction = production;
            ClearContentView(_contentParent);
            InitVariants();
        }

        private void OnVariantClick(JToken prod, JToken variant)
        {
            GameService.GetGameController().PostNewWorld(prod, variant);
            Hide();
        }
        
        private string RenameDescription(string desc, string target, string replaceWord)
        {
            string pattern = @"«(.*?)»";
            
            string result = Regex.Replace(desc, pattern, match =>
            {
                string valueBetweenQuotes = match.Value.Trim('«', '»');
                
                if (valueBetweenQuotes == target)
                    return replaceWord;
                else
                    return match.Value;
            });

            return result;
        }
        
        private void ClearContentView(Transform content)
        {
            if(content.childCount == 0)
                return;
            
            for (var i = content.childCount - 1; i >= 0; i--)
                Destroy(content.GetChild(i).gameObject);
        }
        
        private string GetPolishPart(string title)
        {
            char[] delimiter = {'/'};
        
            string[] words = title.Split(delimiter);

            if (words.Length > 1)
                return words[1].Trim();
            
            return words[0].Trim();
        }
    }
}