namespace TaskB
{
    public class ContentValidator : IContentValidator
    {
        public bool Validate(string content, ItemType type)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            return type switch
            {
                ItemType.JSON => LooksLikeJson(content),
                ItemType.XML => LooksLikeXml(content),
                _ => false
            };
        }

        private static bool LooksLikeJson(string content)
        {
            string trimmed = content.TrimStart();
            return trimmed.StartsWith("{") || trimmed.StartsWith("[");
        }

        private static bool LooksLikeXml(string content)
        {
            string trimmed = content.TrimStart();
            return trimmed.StartsWith("<");
        }
    }
}
