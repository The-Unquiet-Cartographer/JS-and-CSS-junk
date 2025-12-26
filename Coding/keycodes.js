//Keycode references
const   kc_0 = 48,  kc_9 = 57,
        kc_A = 65,  kc_Z = 90, 
        kc_a = 97,  kc_z = 122;

/////////////////////
/* MATCH A KEYCODE */
/////////////////////

//if (e.keyCode === 65 || e.keyCode === 97)         { console.log ("yes"); }    //Gets A or a.
//if (e.key.match(/^(A|a)$/))                       { console.log ("yes"); }    //Gets A or a using a regular expression.


//if (e.key.toUpperCase === 'A')                    { console.log ("yes"); }    //Doesn't seem to work.
//if (['A', 'a'].includes(e.key.toUppercase()))     { console.log ("yes"); }    //Throws an error.
//if (['A', 'a'].includes(e.key.toUppercase))       { console.log ("yes"); }    //Does nothing.
//if (e.keyCode.match(/^(65|97)$/))                 { console.log ("yes"); }    //NOPE. Doesn't work with == either.
