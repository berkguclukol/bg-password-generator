let generateBtn = document.getElementById("GenerateBtn");
let generateBtnBottom = document.getElementById("GenerateBtnBottom");
let copyBtn = document.getElementById("CopyBtn");
let passwordArea = document.getElementById("password");
let range_text = document.getElementById("range-text");
const lengthEl = document.getElementById("length");
const uppercaseEl = document.getElementById("uppercase");
const lowercaseEl = document.getElementById("lowercase");
const numbersEl = document.getElementById("numbers");
const symbolsEl = document.getElementById("symbols");
rangeChange("mousedown");
rangeChange("mousemove");
rangeChange("keydown");
range_text.innerHTML = lengthEl.value;

function rangeChange(evtType) {
  lengthEl.addEventListener(evtType, function () {
    window.requestAnimationFrame(function () {
      range_text.innerHTML = lengthEl.value;
      lengthEl.setAttribute("aria-valuenow", lengthEl.value);
    });

  });
}
const randomFunc = {
  lower: getRandomLower,
  upper: getRandomUpper,
  number: getRandomNumber,
  symbol: getRandomSymbol,
};
copyBtn.addEventListener("click", () => {
  const textarea = document.createElement("textarea");
  const password = passwordArea.innerText;
  if (!password) {
    return;
  }
  textarea.value = password;
  document.body.appendChild(textarea);
  textarea.select();
  document.execCommand("copy");
  textarea.remove();
  copyBtn.innerHTML = `<i class="bi bi-clipboard-check"></i>`;
  copyBtn.setAttribute("disabled", "disabled");
  copyBtn.classList.add("disabled");
  setTimeout(() => {
    copyBtn.innerHTML = `<i class="bi bi-clipboard"></i>`;
    copyBtn.removeAttribute("disabled");
    copyBtn.classList.remove("disabled");
  }, 3000);
  return false;
});


generateBtn.addEventListener("click", () => {
  passwordArea.innerText = generatePassword(
      lowercaseEl.checked,
      uppercaseEl.checked,
      numbersEl.checked,
      symbolsEl.checked,
      +lengthEl.value
  );
});
generateBtnBottom.addEventListener("click", () => {
  passwordArea.innerText = generatePassword(
      lowercaseEl.checked,
      uppercaseEl.checked,
      numbersEl.checked,
      symbolsEl.checked,
      +lengthEl.value
  );
});

function generatePassword(lower, upper, number, symbol, length) {
  let generatedPassword = "";
  const typesCount = lower + upper + number + symbol;
  const typesArr = [{ lower }, { upper }, { number }, { symbol }].filter(
      (item) => Object.values(item)[0]
  );
  if (typesCount === 0) {
    return "";
  }
  for (let i = 0; i < length; i += typesCount) {
    typesArr.forEach((type) => {
      const funcName = Object.keys(type)[0];
      generatedPassword += randomFunc[funcName]();
    });
  }
  const finalPassword = generatedPassword.slice(0, length);
  return finalPassword;
}

function getRandomLower() {
  return String.fromCharCode(Math.floor(Math.random() * 26) + 97);
}

function getRandomUpper() {
  return String.fromCharCode(Math.floor(Math.random() * 26) + 65);
}

function getRandomNumber() {
  return +String.fromCharCode(Math.floor(Math.random() * 10) + 48);
}

function getRandomSymbol() {
  const symbols = "!@#$%^&*(){}[]=<>/,.";
  return symbols[Math.floor(Math.random() * symbols.length)];
}
generateBtn.click();