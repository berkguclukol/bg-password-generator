let generateBtn = document.getElementById("GenerateBtn");
let generateBtnTop = document.getElementById("generateBtnTop");
//let copyBtn = document.getElementById("CopyBtn");
let copyBtnTop = document.getElementById("copyBtnTop");
let passwordArea = document.getElementById("password");
let range_text = document.getElementById("range-text");
const lengthEl = document.getElementById("length");
const uppercaseEl = document.getElementById("uppercase");
const lowercaseEl = document.getElementById("lowercase");
const numbersEl = document.getElementById("numbers");
const symbolsEl = document.getElementById("symbols");
const strengthTextEl = document.getElementById("strengthText");

const alertEl = document.getElementById("alertArea");


rangeChange("mousedown");
rangeChange("mousemove");
rangeChange("keydown");
range_text.innerHTML = `Password length : ${lengthEl.value}`;

function rangeChange(evtType) {
  lengthEl.addEventListener(evtType, function () {
    window.requestAnimationFrame(function () {
      range_text.innerHTML = `Password length : ${lengthEl.value}`;
      lengthEl.setAttribute("aria-valuenow", lengthEl.value);
    });

  });
}
const charSets = {
  lower: "abcdefghijklmnopqrstuvwxyz",
  upper: "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
  number: "0123456789",
  symbol: "!@#$%^&*(){}[]=<>/,.",
};

copyBtnTop.addEventListener("click", () => {
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
  alertEl.classList.remove("hidden");
  setTimeout(() => {
    alertEl.classList.add("hidden");
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
  strengthTextEl.innerHTML = printStrongNess(passwordArea.innerText);
});
generateBtnTop.addEventListener("click", () => {
  passwordArea.innerText = generatePassword(
      lowercaseEl.checked,
      uppercaseEl.checked,
      numbersEl.checked,
      symbolsEl.checked,
      +lengthEl.value
  );
  strengthTextEl.innerHTML = printStrongNess(passwordArea.innerText);
});

function generatePassword(lower, upper, number, symbol, length) {
  const selectedSets = Object.entries({ lower, upper, number, symbol })
      .filter(([, enabled]) => enabled)
      .map(([name]) => charSets[name]);
  if (selectedSets.length === 0) {
    return "";
  }
  const pool = selectedSets.join("");
  // Guarantee at least one character from every selected type
  const chars = selectedSets.map((set) => set[getRandomInt(set.length)]);
  while (chars.length < length) {
    chars.push(pool[getRandomInt(pool.length)]);
  }
  // Fisher-Yates shuffle so character types don't sit at fixed positions
  for (let i = chars.length - 1; i > 0; i--) {
    const j = getRandomInt(i + 1);
    [chars[i], chars[j]] = [chars[j], chars[i]];
  }
  return chars.slice(0, length).join("");
}

// Uniform integer in [0, max) from a CSPRNG, using rejection sampling to avoid modulo bias
function getRandomInt(max) {
  const limit = Math.floor(0x100000000 / max) * max;
  const buffer = new Uint32Array(1);
  do {
    crypto.getRandomValues(buffer);
  } while (buffer[0] >= limit);
  return buffer[0] % max;
}
function printStrongNess(input_string) {
  const n = input_string.length;
  // Checking lower alphabet in string
  let hasLower = false;
  let hasUpper = false;
  let hasDigit = false;
  let specialChar = false;
  const normalChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 ";

  for (let i = 0; i < n; i++) {
    if (input_string[i] >= "a" && input_string[i] <= "z") {
      hasLower = true;
    }
    if (input_string[i] >= "A" && input_string[i] <= "Z") {
      hasUpper = true;
    }
    if (input_string[i] >= "0" && input_string[i] <= "9") {
      hasDigit = true;
    }
    if (!normalChars.includes(input_string[i])) {
      specialChar = true;
    }
  }

  // Strength of password
  let strength = "Weak";
  if (hasLower && hasUpper && hasDigit && specialChar && n >= 8) {
    strength = "Strong";
  } else if ((hasLower || hasUpper) && specialChar && n >= 6) {
    strength = "Moderate";
  }

  return `Strength: ${strength}`;
}
generateBtn.click();