//Default parameters for floating window elements
    const Default = {
        width: 640,
        height: 480,
        anchorHeight: 20,
        borderSize: 8,
        minWidth: 100,
        minHeight: 20
    }

//Stores a reference to the element, the mouse position relative to the element, and the maximum bounds of the element.
    class WindowAnchor {
        constructor(element,eventData,parentRect) {
            this.element = element;
            this.xDiff = eventData.clientX - parentRect.left;
            this.yDiff = eventData.clientY - parentRect.top;
            this.width = parentRect.width;
            this.height = parentRect.height;
        }
    }
//Stores a reference to the element and the parent element, and the mouse position relative to each border.
    class WindowBorder {
        constructor(element,eventData,parentRect) {
            this.element = element;
            this.parent = element.parentElement;
            this.lDiff = undefined;
            this.rDiff = undefined;
            this.tDiff = undefined;
            this.bDiff = undefined;
            const lDiff = eventData.clientX - parentRect.left;
            if (lDiff < 0) this.lDiff = Math.abs(lDiff);
            const rDiff = eventData.clientX - (parentRect.left + parentRect.width);
            if (rDiff > 0) this.rDiff = rDiff;
            const tDiff = eventData.clientY - parentRect.top;
            if (tDiff < 0) this.tDiff = Math.abs(tDiff);
            const bDiff = eventData.clientY - (parentRect.top + parentRect.height);
            if (bDiff > 0) this.bDiff = bDiff;
            //console.log(`${this.lDiff} ${this.rDiff} ${this.tDiff} ${this.bDiff}`);
        }
    }

//Store information pertaining to the element you want to manipulate, using one of the above classes.
    let grabbedObject;



////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  INITIALISE FLOATING WINDOW ELEMENTS
//
////////////////////////////////////////////////////////////////////////////////////////////////////

    const arr_floatingWindows = Array.from(document.getElementsByClassName("floatingWindow"));
    for (let i = 0; i < arr_floatingWindows.length; i++) {
        const fw = arr_floatingWindows[i];

    //Set default height/width
        fw.style.height=px(Default.height);
        fw.style.width=px(Default.width);
        fw.style.position="absolute";

    //Insert anchor element
    /*
    *   Will be used to click and drag the window around the screen.
    */
        fw.insertBefore(
            NewElement("div", ["wFloat-anchor"], (elmt)=>{
            //Initial styling
                elmt.style.width="100%";
                elmt.style.height=px(Default.anchorHeight);
                elmt.style.background="grey";
            //Add event listener
                elmt.addEventListener('mousedown', (e)=>{
                    const rect = fw.getBoundingClientRect();
                    grabbedObject = new WindowAnchor(fw, e, rect);
                });

            }),
            fw.firstChild
        );

    //Insert border element
    /*
    *   Will be used to resize the element
    */
        fw.insertBefore(
            NewElement("div", ["wFloat-border"], (elmt)=>{
            //Initial styling
                elmt.style.width = `calc(100% + ${px(Default.borderSize*2)})`;
                elmt.style.height = `calc(100% + ${px(Default.borderSize*2)})`;
                elmt.style.position = "absolute";
                elmt.style.top = px(-Default.borderSize);
                elmt.style.left = px(-Default.borderSize);
                elmt.style.zIndex = fw.style.zIndex -1;
            //Add event listener
                elmt.addEventListener('mousedown', (e)=>{
                    const rect = fw.getBoundingClientRect();
                    grabbedObject = new WindowBorder(elmt, e, rect);
                });
            //Hover cursor
                elmt.addEventListener('mousemove', (e)=>{
                    const rect = elmt.getBoundingClientRect();
                    let flags = parseInt([
                        e.clientX < rect.left + Default.borderSize ? 1:0,
                        e.clientX > rect.right - Default.borderSize ? 1:0,
                        e.clientY > rect.bottom - Default.borderSize ? 1:0,
                        e.clientY < rect.top + Default.borderSize ? 1:0
                    ].join(""), 2);
                    //console.log (flags);
                    switch(flags) {
                        default: elmt.style.cursor = "default"; break;
                        case 1: elmt.style.cursor = "n-resize"; break;
                        case 2: elmt.style.cursor = "s-resize"; break;
                        case 4: elmt.style.cursor = "e-resize"; break;
                        case 8: elmt.style.cursor = "w-resize"; break;
                        case 5: elmt.style.cursor = "ne-resize"; break;
                        case 6: elmt.style.cursor = "se-resize"; break
                        case 9: elmt.style.cursor = "nw-resize"; break; 
                        case 10: elmt.style.cursor = "sw-resize"; break;
                    }
                });
                elmt.addEventListener('mouseleave', ()=>{elmt.style.cursor = "default";});
            }),
            fw.firstChild
        );
    }



////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  CLICK & DRAG EVENTS
//
////////////////////////////////////////////////////////////////////////////////////////////////////

    document.addEventListener('mousemove', (e)=>{
        if (grabbedObject) {
            if (grabbedObject.element.classList.contains("wFloat-border")) ResizeWindow(e);
            else DragWindow(e);
        }
    });
    document.addEventListener('mouseup', ()=>{grabbedObject = undefined;});
    //elmt.addEventListener('mouseleave', ()=>{grabbedObject = undefined;});
    //elmt.addEventListener('mouseout', ()=>{grabbedObject = undefined;});


//Resize a window by its borders
    function DragWindow(e) {
        newPos_x = px((e.clientX - grabbedObject.xDiff).clamp(0, window.innerWidth - grabbedObject.width));
        newPos_y = px((e.clientY - grabbedObject.yDiff).clamp(0, window.innerHeight - grabbedObject.height));
        grabbedObject.element.style.left = newPos_x;
        grabbedObject.element.style.top = newPos_y;
        //console.log (newPos_x+" "+newPos_y);
    }


//Resize a window by its borders
/*
*   lDiff & tDiff are given absolute values 
*   We clamp the client x/y (mouse position) to prevent us from exceeding the box bounds
*/
    function ResizeWindow(e) {
        const rect = grabbedObject.parent.getBoundingClientRect();
        if (grabbedObject.lDiff) {
            let _clientX = e.clientX.clamp(grabbedObject.lDiff, rect.right - Default.minWidth - grabbedObject.lDiff);
            grabbedObject.parent.style.width = px(rect.right - _clientX - grabbedObject.lDiff);
            grabbedObject.parent.style.left = px(_clientX + grabbedObject.lDiff);
        }
        else if (grabbedObject.rDiff) {
            let _clientX = e.clientX.clamp(rect.left + Default.minWidth + grabbedObject.rDiff, window.innerWidth - grabbedObject.rDiff);
            grabbedObject.parent.style.width = px(_clientX - grabbedObject.rDiff - rect.left);
        }
        if (grabbedObject.tDiff) {
            let _clientY = e.clientY.clamp(grabbedObject.tDiff, rect.bottom - Default.minHeight - grabbedObject.tDiff);
            grabbedObject.parent.style.height = px(rect.bottom - _clientY - grabbedObject.tDiff);
            grabbedObject.parent.style.top = px(_clientY + grabbedObject.tDiff);
        }
        else if (grabbedObject.bDiff) {
            let _clientY = e.clientY.clamp(rect.top + Default.minHeight + grabbedObject.bDiff, window.innerHeight - grabbedObject.bDiff);
            grabbedObject.parent.style.height = px(_clientY - grabbedObject.bDiff - rect.top);
        }
        //console.log (newPos_x+" "+newPos_y);
    }



////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  UTILITY FUNCTIONS
//
////////////////////////////////////////////////////////////////////////////////////////////////////

    Number.prototype.clamp = function(min, max) {
        return Math.min(Math.max(this, min), max);
    };
    function px(num){return `${num}px`};
