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

}
  
  
  