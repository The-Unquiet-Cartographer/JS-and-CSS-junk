const Dice = {

    Roll_1: function (minResult, maxResult) {
        maxResult = Math.floor(maxResult) + 1;              //<== Add 1 to max because Math.floor(random * range) will always return < max.
        minResult = Math.floor(minResult);                  //  If Math.floor(random * range) === 0 function returns min;
        return (Math.floor(Math.random() * (maxResult - minResult)) + minResult);
    },

//Get a die roll that is biased towards the middle of the range like a 2D roll would be.
    Roll_2: function (minResult, maxResult) {
        const a = Maff.Die_roll (minResult, maxResult);
        const b = Maff.Die_roll (minResult, maxResult + 1);
        return Math.ceil( ((a + b) - 1) / 2);                       // It works, ok? Just trust me.
        //OR 
        //return Math.ceil( ((a + b) / 2) - 0.5);
    },

    Roll_X: function (dice, dieMin, dieMax) {
        let result;
        for (let i=0; i<dice; i++) {
            result += Die_roll(dieMin, dieMax);
        }
        return result;
    },

}