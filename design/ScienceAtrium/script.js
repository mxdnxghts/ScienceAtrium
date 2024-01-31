if (document.documentElement.clientWidth > 539) {

  document.addEventListener('DOMContentLoaded', () => {
    const main = document.querySelector('.main');
    const header = document.querySelector('.header');
  
  const headerFixed = () => {
    let scrollTop = window.scrollY;
    let mainCenter = main.offsetHeight / 2;
  
    if (scrollTop >= mainCenter) {
      header.classList.add('fixed')
      main.style.marginTop = `${header.offsetHeight}px`;
      header.style.background = `rgba(178, 196, 188, 1)`;
    } else {
      header.classList.remove('fixed')
      main.style.marginTop = `0px`;
      
    }
  };
  
  headerFixed();
    window.addEventListener('scroll', () => {
      headerFixed();
    });
  });

};




const selectedAll = document.querySelectorAll(".wrapper-dropdown");

selectedAll.forEach((selected) => {
  const optionsContainer = selected.children[2];
  const optionsList = selected.querySelectorAll("div.wrapper-dropdown li");

  selected.addEventListener("click", () => {
    let arrow = selected.children[1];

    if (selected.classList.contains("active")) {
      handleDropdown(selected, arrow, false);
    } else {
      let currentActive = document.querySelector(".wrapper-dropdown.active");

      if (currentActive) {
        let anotherArrow = currentActive.children[1];
        handleDropdown(currentActive, anotherArrow, false);
      }

      handleDropdown(selected, arrow, true);
    }
  });

  // update the display of the dropdown
  for (let o of optionsList) {
    o.addEventListener("click", () => {
      selected.querySelector(".selected-display").innerHTML = o.innerHTML;
    });
  }
});

// check if anything else ofther than the dropdown is clicked
window.addEventListener("click", function (e) {
  if (e.target.closest(".wrapper-dropdown") === null) {
    closeAllDropdowns();
  }
});

// close all the dropdowns
function closeAllDropdowns() {
  const selectedAll = document.querySelectorAll(".wrapper-dropdown");
  selectedAll.forEach((selected) => {
    const optionsContainer = selected.children[2];
    let arrow = selected.children[1];

    handleDropdown(selected, arrow, false);
  });
}

// open all the dropdowns
function handleDropdown(dropdown, arrow, open) {
  if (open) {
    arrow.classList.add("rotated");
    dropdown.classList.add("active");
  } else {
    arrow.classList.remove("rotated");
    dropdown.classList.remove("active");
  }
}