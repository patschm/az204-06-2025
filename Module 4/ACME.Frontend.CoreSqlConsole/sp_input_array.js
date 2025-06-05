function sample(arr) {
    if (typeof arr === "string") arr = JSON.parse(arr);

    arr.forEach(function (a) {
        // do something here
        console.log(a);
    });
}