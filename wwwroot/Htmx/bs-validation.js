htmx.defineExtension('bs-validation', {
    onEvent: function (name, evt, data) {

      if (name !== "htmx:afterProcessNode") {
            return;
        }
      console.log('event ' + name);

      let form = evt.detail.elt;

          // check if trigger attribute and submit event exists
      // for the form
      if(!form.hasAttribute('hx-trigger')){
          // set trigger for custom event bs-send
        form.setAttribute('hx-trigger','bs-send');
              // and attach the event only once
        form.addEventListener('submit', function (event) {
          if (form.checkValidity()) {
            // trigger custom event hx-trigger="bs-send"
            htmx.trigger(form, "bsSend");
            console.log('bsSend')
          }

          console.log('prevent')
          event.preventDefault()
          event.stopPropagation()

          form.classList.add('was-validated')
        }, false)
      }
    }
  });