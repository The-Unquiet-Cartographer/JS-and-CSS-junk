//Create a new element, with id, classes, and styles
/*
*   Hint: Prefix a style with '#' to set an ID.
*/
    function NewElement(elmtType, Arr_classes = [], Func_appendStyles = (_newElmt)=>{return;}) {        //<== With Func_appendStyles you can add a function to the constructor like (_newElmt)=>{DOSTUFF}). Useful for including extra paramaters.
        const newElmt = document.createElement(elmtType);
        for (cl in Arr_classes) {
            if (cl[0] === '#') newElmt.id = cl.substring(1);
            else newElmt.classList.add(Arr_classes[cl]);
        }
        Func_appendStyles(newElmt);
        return newElmt;
    }