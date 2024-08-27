const RECIPES = [];

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

function navigateToRecipeList() {
    window.location.href = 'home.html';
}

function navigateToRandomRecipe() {
    let hasLinkFilterValue = document.getElementById("hasLinkCheckbox").checked;
    let cuisineFilterValue = document.getElementById("cuisineSelect").value;
    let proteinFilterValue = document.getElementById("proteinSelect").value;
    let cookingTimeFilterValue = document.getElementById("cookingTimeSelect").value;

    var filteredRecipes = [...RECIPES];
    if (hasLinkFilterValue) {
        filteredRecipes = RECIPES.filter(x => x.IsOnlineRecipe);
    }
    if (cuisineFilterValue !== "Choose a cuisine") {
        filteredRecipes = filteredRecipes.filter(x => x.Cuisine == cuisineFilterValue);
    }
    if (proteinFilterValue !== "Choose a protein") {
        filteredRecipes = filteredRecipes.filter(x => x.Protein == proteinFilterValue);
    }
    if (cookingTimeFilterValue !== "-1") {
        let cookingTimeMin =  parseInt(cookingTimeFilterValue.split(",")[0]);
        let cookingTimeMax = parseInt(cookingTimeFilterValue.split(",")[1]);
        filteredRecipes = filteredRecipes.filter(x => x.CookingTimeInMinutes >= cookingTimeMin && x.CookingTimeInMinutes <= cookingTimeMax);
    }

    if (filteredRecipes.length == 0) {
        window.alert("No recipe found matching those filters.");
        return;
    }

    let recipeIndex = Math.floor(Math.random() * filteredRecipes.length);
    let recipeLink = filteredRecipes[recipeIndex].Link;

    window.location.href = recipeLink;
}

function navigateToIndex() {
    window.location.href = 'index.html';
}