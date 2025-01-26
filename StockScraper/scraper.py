import requests
import smtplib
from selenium import webdriver
import time
import sys
import numpy as np
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

def main():
    data = np.zeros(len(sys.argv) - 2)
    
    # Ensure arguments
    if len(sys.argv) < 2:
        print("Need args")
        sys.exit(1)
    
    # Set Chrome options for headless mode
    chrome_options = Options()
    chrome_options.add_argument("--headless")
    chrome_options.add_argument("--disable-gpu")
    chrome_options.add_argument("--window-size=1920,1080")
    chrome_options.add_argument("--no-sandbox")
    chrome_options.add_argument("--disable-dev-shm-usage")
    chrome_options.add_argument("--disable-extensions")
    chrome_options.add_argument("--disable-blink-features=AutomationControlled")
    
    # Initialize browser
    driver = webdriver.Chrome(options=chrome_options)

    # NAS driver.get("https://www.nasdaq.com/market-activity/stocks/screener")
    driver.get("https://www.cnbc.com/")

    time.sleep(5)
    
    # Get search box
    driver.execute_script("window.scrollBy(0,1000)") #scroll
    time.sleep(2)
    search_box = driver.find_element(By.CLASS_NAME, "QuoteFinder-searchInput")
    # NAS search_box = driver.find_element(By.CLASS_NAME, "jupiter22-find-symbol__input")

    # Handle each symbol
    for i in range(1, len(sys.argv)):
        symbol = sys.argv[i]  # Get symbol
        search_box.send_keys(symbol)  # Search for symbol
        search_box.send_keys(Keys.RETURN)  # Press Enter
        time.sleep(5)

        driver.execute_script("window.scrollBy(0,300);")
        
        time.sleep(2)
        # Wait for "Share Volume" to load
        try:
            WebDriverWait(driver, 10).until(
                EC.presence_of_element_located((By.CLASS_NAME, "QuoteStrip-volume"))
            )
            share_volume = driver.find_element(By.CLASS_NAME, "QuoteStrip-volume").text
            print("Share Volume:", share_volume)
            data[i-1] = int(share_volume.replace(",", "")) # convert to int and store in data
        except Exception as e:
            print("Element not found:", e)
    return data

if __name__ == "__main__":
    main()
