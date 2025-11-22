//CREATE A WORLD MAP, POPULATE IT WITH TILES (in-game screens), SET THE STATE OF EACH REGION (e.g. seasons, isDestroyed, etc.)
    class Map {

    //Style functions
    /*
    *   Using width/height, NOT style.width/style.height. This is because we're applying it to <canvas> elements, which work a bit differently.
    */
        static #WidthHeightLeftTop(width, height, left, top) {
            return((_elmt)=>{_elmt.width = width; _elmt.height = height; _elmt.style.left = `${left}px`; _elmt.style.top = `${top}px`;});
        }
        static #WidthHeight(width, height) {
            return((_elmt)=>{_elmt.width = width; _elmt.height = height;});
        }
        static #LeftTop(left, top) {
            return((_elmt)=>{_elmt.style.left = `${left}px`; _elmt.style.top = `${top}px`;});
        }


    //Constructor
        constructor (elmt, screens_x, screens_y) {
            this.DOMElement = elmt;
            this.width = screens_x;
            this.height = screens_y;
            this.DOMElement.style.width = `${this.width * tilePixels_x}px`;
            this.DOMElement.style.height = `${this.height * tilePixels_y}px`;
            this.screenCtnr = elmt.appendChild(NewElement("span", [`#${this.DOMElement.id}-screenCtnr`]));
            this.markerCtnr = elmt.appendChild(NewElement("span", [`#${this.DOMElement.id}-markerctnr`]));
            this.overlayCanvas = this.DOMElement.appendChild(NewElement("canvas", [`#${this.DOMElement.id}-overlay`], Map.#WidthHeight(this.width * tilePixels_x, elmt.height = this.height * tilePixels_y)));
        //Add canvases for each screen
            for (let y=0; y<this.height; y++) {
                for (let x=0; x<this.width; x++) {
                    this.screenCtnr.appendChild(NewElement("canvas", ["tile", "shroud"], Map.#WidthHeightLeftTop(tilePixels_x, tilePixels_y, x * tilePixels_x, y * tilePixels_y)));
                }
            }
        }

    //Add entrances
        AddEntrances(arr_entrances=[]) {
            arr_entrances.forEach((entr)=>{
                const newMarker = this.markerCtnr.appendChild(NewElement("span", ["marker"], Map.#LeftTop(entr.pos_x+8, entr.pos_y+8)));
                newMarker.appendChild(NewElement("span", ["marker-ping"]));
            });
        }


    //Draw each screen in a region, snapshotted from a source image
    /*
    *   area_index0 is an index modifier to be applied when the source image is smaller than the world map.
        e.g. The Natzu source images are cropped, so the first screen is index 0. On the world map, however, the first screen is at index 70. So we set area_index0 to 70. 
    */
        SetAreaState(arr_area_tileIndices=[], srcElmt_mapImage, area_index0 = 0) {
            arr_area_tileIndices.forEach((i)=>{
                const moddedIndex = i - area_index0;
                const src_y = Math.floor((moddedIndex)/this.width);
                const src_x = (moddedIndex)%this.width;
                const dest_canvas = this.screenCtnr.children[i];
                const context = dest_canvas.getContext('2d');
                context.drawImage(srcElmt_mapImage, src_x*tilePixels_x, src_y*tilePixels_y, tilePixels_x, tilePixels_y, 0, 0, tilePixels_x, tilePixels_y);
            });
        }

    //Give the main DOM element an offset.
        Offset(x, y) {
            if (x!=0) this.DOMElement.style.left = `${x}px`;
            if (y!=0) this.DOMElement.style.top = `${y}px`;
        }
    }


//STORE THE NAME OF A REGION, THE TILE INDICES THAT MAKE IT UP, AND ITS CURRENT STATE
    class RegionData {
        constructor (id, tileIndices = [], state) {
            this.id = id;
            this.tileIndices = tileIndices;
            this.state = state;
        }
    }


//STORE THE NAME, POSITION, AND TYPE OF AN ENTRANCE
    class EntranceData {
        static type = {
            single: 0,
            connector: 1,
            portal: 2,
            generic: 3,
            special: 4
        }
        constructor (name, type, pos_x, pos_y, notes) {
            this.name = name;
            this.type = type;
            this.pos_x = pos_x;
            this.pos_y = pos_y;
            this.notes = notes;
        }
    }