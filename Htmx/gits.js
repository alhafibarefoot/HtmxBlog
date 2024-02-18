/*
  Alpine Dev Tools: https://github.com/Te7a-Houdini/alpinejs-devtools
*/
function gistsData() {
  return {
    title: 'Latest Gists',
    gists: [],
    reload() {
      sessionStorage.removeItem("gists");
      this.gists = [];
      this.init();
    },
    init() {
      // Testdata, in case I hit my 60 calls per hour
      /*let gists = [
          {
          "id": "8f6af49ffe693c15faca67a7f3bf1a31",
          "html_url": "https://gist.github.com/8f6af49ffe693c15faca67a7f3bf1a31",
          "description": "Test"
          }
      ];*/


      // Check if sessionData holds anything, so we don't need to hit the api
      const gists = JSON.parse(sessionStorage.getItem("gists"));

      if(gists){
        // make it accessible to x-data
        this.gists = gists;
        console.log('sessionStorage', gists);
        return;
      }

      console.log(new leptonParser().parse('[bla] #blub'))

      // get gists and parse the description field
      fetch('https://api.github.com/users/marcus-at-localhost/gists')
        .then(response => response.json())
        .then(response => {
          console.log('fetched',response);
          // I could use collect.js to manipulate the response further.
          let gists = response.map((item) => {
            // parser: https://codepen.io/localhorst/pen/ZEbqVZd
            item.parsed = new leptonParser().parse(item.description);
            return item;
          });

          this.gists = gists;
          sessionStorage.setItem("gists",JSON.stringify(gists));
          console.log(this,response)
      });
    }
  };
}