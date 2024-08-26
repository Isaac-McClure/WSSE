function filterList() {
    let filterElement = document.getElementById("proteinFilter");
    let filterValue = filterElement.value;

    let listElement = document.getElementById("recipeList");

    let listItems = listElement.children;

    let showDisplay = "list-item";
    let hideDisplay = "none";

    // Filter the array
    if (filterValue == "Choose a protein")
    {
        for (const listItem of listItems)
        {
            listItem.style.display = showDisplay;
        }
    }
    else
    {
        for (const listItem of listItems)
        {
            if (listItem.getAttribute("data-protein") == filterValue)
            {
                listItem.style.display = showDisplay;
            }
            else
            {
                listItem.style.display = hideDisplay;
            }
        }
    }
}