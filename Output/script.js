const RECIPES = [{ Name: "Zucchini and butter bean fritters",
LinkOrBook: "Modern Australian Vegan - pg 82", 
Cuisine: "Mediterranian", 
Protein: "Beans", 
CookingTimeInMinutes: 35, 
Link: "recipe0.html", 
IsOnlineRecipe: false},
{ Name: "Smoky white bean and tomato soup",
LinkOrBook: "Modern Australian Vegan - pg 100", 
Cuisine: "Soup", 
Protein: "Beans", 
CookingTimeInMinutes: 120, 
Link: "recipe1.html", 
IsOnlineRecipe: false},
{ Name: "1-pot spiced sweet potato lentil soup",
LinkOrBook: "https://minimalistbaker.com/1-pot-spiced-sweet-potato-lentil-soup/", 
Cuisine: "Soup", 
Protein: "Lentils", 
CookingTimeInMinutes: 50, 
Link: "recipe2.html", 
IsOnlineRecipe: true},
{ Name: "Vegetable enchiladas with a roasted tomato sauce",
LinkOrBook: "Modern Australian Vegan - pg 174", 
Cuisine: "Mexican", 
Protein: "Beans", 
CookingTimeInMinutes: 160, 
Link: "recipe3.html", 
IsOnlineRecipe: false},
{ Name: "Sesame noodles",
LinkOrBook: "Modern Australian Vegan - pg 204", 
Cuisine: "Asian", 
Protein: "None", 
CookingTimeInMinutes: 20, 
Link: "recipe4.html", 
IsOnlineRecipe: false},
{ Name: "Polenta with spicy tomato-mushroom ragu",
LinkOrBook: "100 best vegan recipes - pg 97", 
Cuisine: "Italian", 
Protein: "Mushroom", 
CookingTimeInMinutes: 45, 
Link: "recipe5.html", 
IsOnlineRecipe: false},
{ Name: "Bok choy and ginger-sesame udon noodles",
LinkOrBook: "100 best vegan recipes- pg 123", 
Cuisine: "Asian", 
Protein: "None", 
CookingTimeInMinutes: 20, 
Link: "recipe6.html", 
IsOnlineRecipe: false},
{ Name: "roast pumpkin, caper and blue cheese pasta",
LinkOrBook: "The easy kitchen - pg 108", 
Cuisine: "Pasta", 
Protein: "None", 
CookingTimeInMinutes: 70, 
Link: "recipe7.html", 
IsOnlineRecipe: false},
{ Name: "butternut pumpkin pasta bake",
LinkOrBook: "The easy kitchen - pg 140", 
Cuisine: "Pasta", 
Protein: "Mushroom", 
CookingTimeInMinutes: 55, 
Link: "recipe8.html", 
IsOnlineRecipe: false},
{ Name: "Baked creamy feta risoni",
LinkOrBook: "a screenshot from my mother", 
Cuisine: "Pasta", 
Protein: "Cheese", 
CookingTimeInMinutes: 45, 
Link: "recipe9.html", 
IsOnlineRecipe: false},
{ Name: "Giant pasta with mushroom sauce",
LinkOrBook: "Cooking with kindness - pg 122", 
Cuisine: "Pasta", 
Protein: "Mushroom", 
CookingTimeInMinutes: 45, 
Link: "recipe10.html", 
IsOnlineRecipe: false},
{ Name: "pumpkin soup",
LinkOrBook: "brain", 
Cuisine: "Soup", 
Protein: "None", 
CookingTimeInMinutes: 0, 
Link: "recipe11.html", 
IsOnlineRecipe: false},
{ Name: "Burritos",
LinkOrBook: "brain", 
Cuisine: "Mexican", 
Protein: "Beans", 
CookingTimeInMinutes: 0, 
Link: "recipe12.html", 
IsOnlineRecipe: false},
{ Name: "Miso noodle soup",
LinkOrBook: "brain", 
Cuisine: "Asian", 
Protein: "Tofu", 
CookingTimeInMinutes: 15, 
Link: "recipe13.html", 
IsOnlineRecipe: false},
{ Name: "mediterranian wraps",
LinkOrBook: "brain", 
Cuisine: "Mediterranian", 
Protein: "Tofu", 
CookingTimeInMinutes: 20, 
Link: "recipe14.html", 
IsOnlineRecipe: false},
{ Name: "Garlic fried rice",
LinkOrBook: "https://naturallieplantbased.com/garlic-fried-rice/", 
Cuisine: "Asian", 
Protein: "Tofu", 
CookingTimeInMinutes: 30, 
Link: "recipe15.html", 
IsOnlineRecipe: true},
{ Name: "Turkish bread, falafel and dip",
LinkOrBook: "brain", 
Cuisine: "Turkish", 
Protein: "Hummus", 
CookingTimeInMinutes: 5, 
Link: "recipe16.html", 
IsOnlineRecipe: false},
{ Name: "Poke bowl",
LinkOrBook: "?", 
Cuisine: "Asian", 
Protein: "Tofu", 
CookingTimeInMinutes: 0, 
Link: "recipe17.html", 
IsOnlineRecipe: false},
{ Name: "Roast veggies",
LinkOrBook: "brain", 
Cuisine: "", 
Protein: "None", 
CookingTimeInMinutes: 45, 
Link: "recipe18.html", 
IsOnlineRecipe: false}];

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